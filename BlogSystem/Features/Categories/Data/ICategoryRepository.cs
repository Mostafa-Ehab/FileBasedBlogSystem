using BlogSystem.Domain.Entities;

namespace BlogSystem.Features.Categories.Data;

public interface ICategoryRepository
{
    Category? GetCategoryBySlug(string slug);
    Category[] GetAllCategories();
    Category CreateCategory(Category category);
    Category UpdateCategory(Category category);
    void DeleteCategory(Category category);
    bool CategoryExists(string slug);
}