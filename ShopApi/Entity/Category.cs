namespace ShopApi.Entity;

public class Category
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int? ParentCategoryId { get; set; }
    public required Category ParentCategory { get; set; }
    public required List<Category> SubCategories { get; set; }
    public required string ImageUrl { get; set; }
}