using BlogSystem.Domain.Entities;
using BlogSystem.Features.Categories.GetCategory.DTOs;

namespace BlogSystem.Features.Categories.GetCategory
{
    public interface IGetCategoryHandler
    {
        Task<Category> GetCategoryAsync(string slug);
        Task<Post[]> GetPostsByCategoryAsync(string categorySlug);
        Task<CategoryDTO[]> GetAllCategoriesAsync();
    }
}