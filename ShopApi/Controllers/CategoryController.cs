using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.FormModels;
using ShopApi.Identity;

namespace ShopApi.Controllers;

public class CategoryController(Database database) : ControllerBase
{
    [HttpGet("/categories")]
    public IActionResult GetCategories()
    {
        var result = database.CategoryRepository.GetAllCategories();
        if (result.Any())
            return Ok(result);
        return NotFound();
    }

    [HttpGet("/categories/id={id:int}")]
    public IActionResult GetCategoriesByParentId(int id)
    {
        var result = database.CategoryRepository.GetCategoriesByParentCategoryId(id);
        if (result.Any())
            return Ok(result);
        return NotFound();
    }

    [Authorize(Policy = IdentityData.AdminUserPolicyName)]
    [HttpPost("/categories")]
    public IActionResult CreateCategory([FromBody] CategoryCreateRequest category)
    {
        var categories = database.CategoryRepository.GetAllCategories();
        if (categories.Any(c => c.Name == category.Name))
            return BadRequest("Category already exists");
        var result = database.CategoryRepository.AddCategory(category);
        if (result > 0)
            return Ok();
        return BadRequest();
    }

    [Authorize(Policy = IdentityData.AdminUserPolicyName)]
    [HttpPost("/category/add/preview")]
    public IActionResult AddPreview([FromBody] CategoryPreviewCreateRequest categoryPreviewCreateRequest)
    {
        // Check if category preview already exists
        var categoryPreview = database.CategoryRepository.IsCategoryPreviewExists(categoryPreviewCreateRequest.CategoryId);
        if (categoryPreview > 0)
            database.CategoryRepository.DeleteCategoryPreview(categoryPreviewCreateRequest.CategoryId);
        var result = database.CategoryRepository.AddCategoryPreview(categoryPreviewCreateRequest);
        if (result > 0)
            return Ok();
        return BadRequest();
    }

    [HttpGet("/category/id={id:int}/preview")]
    public IActionResult GetCategoryPreview(int id)
    {
        var result = database.CategoryRepository.GetCategoryPreview(id);
        if (result.Any())
            return Ok(result);
        return NotFound();
    }

    [Authorize(Policy = IdentityData.AdminUserPolicyName)]
    [HttpDelete("/category/id={id:int}")]
    public IActionResult DeleteCategory(int id)
    {
        var result = database.CategoryRepository.DeleteCategory(id);
        if (result > 0)
            return Ok();
        return BadRequest();
    }

    [Authorize(Policy = IdentityData.AdminUserPolicyName)]
    [HttpPost("/category/update")]
    public IActionResult UpdateCategory([FromBody] CategoryUpdateRequest categoryUpdateRequest)
    {
        var result = database.CategoryRepository.UpdateCategory(categoryUpdateRequest);
        if (result > 0)
            return Ok();
        return BadRequest();
    }

    /*
    [Authorize(Policy = IdentityData.AdminUserPolicyName)]
    [HttpPost("/category/preview/update")]
    public IActionResult UpdateCategoryPreview([FromBody] CategoryPreviewUpdateRequest categoryPreviewUpdateRequest)
    {
        var result = database.UpdateCategoryPreview(categoryPreviewUpdateRequest);
        if (result > 0)
            return Ok();
        return BadRequest();
    }*/
}