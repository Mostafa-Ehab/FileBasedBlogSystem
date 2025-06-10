using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Models;

namespace BlogSystem.Features.Categories.GetCategory
{
    public interface IGetCategoryHandler
    {
        Task<Category> GetCategoryAsync(string slug);
        Task<Post[]> GetPostsByCategoryAsync(string categorySlug);
        Task<Category[]> GetAllCategoriesAsync();
    }
}