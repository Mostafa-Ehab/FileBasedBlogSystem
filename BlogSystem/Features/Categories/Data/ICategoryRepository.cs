using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Models;

namespace BlogSystem.Features.Categories.Data
{
    public interface ICategoryRepository
    {
        Category? GetCategoryBySlug(string slug);
        Category[] GetAllCategories();
    }
}