using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Dto;
using ShopApi.Entity;
using ShopApi.FormModels;
using ShopApi.Identity;

namespace ShopApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DataController(Database database, FileService fileService) : ControllerBase
{
    [HttpGet("/addresses")]
    public IActionResult GetAddresses()
    {
        var result = database.GetAllShippingAddress();
        if (result.Any())
            return Ok(result);
        return NotFound();
    }

    [HttpGet("/files")]
    public IActionResult GetFiles()
    {
        var result = database.FileRepository.GetFiles();
        return Ok(result);
    }

    [HttpGet("/files/{id:int}")]
    public IActionResult GetFileById(int id)
    {
        var result = fileService.ResolveFileName(id);
        if (result is null)
            return NotFound();
        return PhysicalFile(result.Value.Item1, result.Value.Item2);
    }

    [Authorize(Policy = IdentityData.AdminUserPolicyName)]
    [HttpPost("/files")]
    public async Task<IActionResult> AddFile(IFormFile formFile)
    {
        var tempFilePath = Path.GetTempFileName();
        try
        {
            await using var stream = System.IO.File.Create(tempFilePath);
            await formFile.CopyToAsync(stream);
            stream.Close();
            var fileId = await fileService.CreateNewFile(tempFilePath, formFile.FileName, formFile.ContentType);
            return Ok(fileId);
        }
        finally
        {
            System.IO.File.Delete(tempFilePath);
        }
    }
}