using BlogSystem.Domain.Entities;
using BlogSystem.Features.Categories.GetCategory.DTOs;
using BlogSystem.Features.Posts.PostManagement.DTOs;

namespace BlogSystem.Features.Categories.GetCategory;

public interface IGetCategoryHandler
{
    Task<Category> GetCategoryAsync(string slug);
    Task<PostResponseDTO[]> GetPostsByCategoryAsync(string categorySlug, int page = 1, int pageSize = 10);
    Task<CategoryDTO[]> GetAllCategoriesAsync();
}