using BlogSystem.Domain.Entities;

namespace BlogSystem.Features.Categories.Data
{
    public interface ICategoryRepository
    {
        Category? GetCategoryBySlug(string slug);
        Category[] GetAllCategories();
    }
}