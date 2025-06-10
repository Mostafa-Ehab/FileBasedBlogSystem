using BlogSystem.Domain.Entities;

namespace BlogSystem.Features.Categories.GetCategory
{
    public interface IGetCategoryHandler
    {
        Task<Category> GetCategoryAsync(string slug);
        Task<Post[]> GetPostsByCategoryAsync(string categorySlug);
        Task<Category[]> GetAllCategoriesAsync();
    }
}