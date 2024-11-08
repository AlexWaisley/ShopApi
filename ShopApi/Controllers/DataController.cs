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
    [HttpGet("/categories")]
    public IActionResult GetCategories()
    {
        var result = database.GetAllCategories();
        if (result.Any())
            return Ok(result);
        return NotFound();
    }

    [HttpGet("/categories/id={id:int}")]
    public IActionResult GetCategoriesByParentId(int id)
    {
        var result = database.GetCategoriesByParentCategoryId(id);
        if (result.Any())
            return Ok(result);
        return NotFound();
    }

    [Authorize(Policy = IdentityData.AdminUserPolicyName)]
    [HttpPost("/categories")]
    public IActionResult CreateCategory([FromBody] CategoryCreateRequest category)
    {
        var result = database.AddCategory(category);
        if (result > 0)
            return Ok();
        return BadRequest();
    }

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
        var result = database.GetFiles();
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