using ShopApi.Dto;
using ShopApi.Entity;
using ShopApi.FormModels;

namespace ShopApi.Data.Categories;

public interface ICategoryRepository
{
    IEnumerable<CategoryDto> GetAllCategories();
    IEnumerable<CategoryDto> GetCategoriesByParentCategoryId(int parentCategoryId);
    int AddCategory(CategoryCreateRequest categoryCreateRequest);
    int DeleteCategory(int id);
    int AddCategoryPreview(CategoryPreviewCreateRequest categoryPreviewCreateRequest);
    void DeleteCategoryPreview(int id);
    IEnumerable<CategoryImage> GetCategoryPreview(int id);
    public int UpdateCategory(CategoryUpdateRequest categoryUpdateRequest);
    public int IsCategoryPreviewExists(int categoryId);


}