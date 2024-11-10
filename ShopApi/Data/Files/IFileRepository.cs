using ShopApi.Entity;

namespace ShopApi.Data.Files;

public interface IFileRepository
{
    IEnumerable<DbFile> GetFiles();
    DbFile? GetFileById(int id);
    int AddFile(long size, string name, string sha256, string contentType);
    int? GetFileByHash(string hash);
}