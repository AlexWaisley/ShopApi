using System.Security.Cryptography;

namespace ShopApi;

public class FileService(IConfiguration configuration, Database database)
{
    public async Task<int> CreateNewFile(string tempFilePath,string fileName, string contentType)
    {
        var filesPath = configuration["FilesPath"];

        if (filesPath is null)
            throw new NullReferenceException("FilesPath is null");

        var fileInfo = new FileInfo(tempFilePath);
        var size = fileInfo.Length;
        await using var stream = fileInfo.OpenRead();
        var hash = await CalculateHashAsync(stream);

        var hashResult = database.GetFileByHash(hash);
        if (hashResult != null)
            return (int)hashResult;
        
        var result = database.AddFile(size, fileName, hash, contentType);

        var fileDir = Path.Combine(filesPath, result.ToString());

        Directory.CreateDirectory(fileDir);
        
        var filePath = Path.Combine(fileDir,fileName);

        await using var fileStream = File.OpenWrite(filePath);

        await stream.CopyToAsync(fileStream);
        
        return result;
    }

    private static async ValueTask<string> CalculateHashAsync(Stream stream, CancellationToken token = default)
    {
        using var sha256 = SHA256.Create();
        stream.Seek(0, SeekOrigin.Begin);
        var hash = await sha256.ComputeHashAsync(stream, token);
        stream.Seek(0, SeekOrigin.Begin);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public (string,string)? ResolveFileName(int id)
    {
        var dbFile = database.GetFileById(id);
        if (dbFile is null)
            return null;
        
        var filesPath = configuration["FilesPath"];

        if (filesPath is null)
            throw new NullReferenceException("FilesPath is null");

        var fileDir = Path.Combine(filesPath, dbFile.Id.ToString());
        var filePath = Path.Combine(fileDir,dbFile.Name);

        return (filePath,dbFile.ContentType);
    }
}