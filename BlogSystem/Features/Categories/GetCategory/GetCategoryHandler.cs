using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Enums;
using BlogSystem.Features.Categories.Data;
using BlogSystem.Features.Categories.GetCategory.DTOs;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Posts.PostManagement.DTOs;
using BlogSystem.Features.Users.Data;
using BlogSystem.Shared.Exceptions.Categories;
using BlogSystem.Shared.Mappings;

namespace BlogSystem.Features.Categories.GetCategory;

public class GetCategoryHandler : IGetCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;

    public GetCategoryHandler(ICategoryRepository categoryRepository, IPostRepository postRepository, IUserRepository userRepository)
    {
        _categoryRepository = categoryRepository;
        _postRepository = postRepository;
        _userRepository = userRepository;
    }

    public Task<Category> GetCategoryAsync(string slug)
    {
        Category? category = _categoryRepository.GetCategoryBySlug(slug);
        if (category == null)
        {
            throw new CategoryNotFoundException(slug);
        }
        return Task.FromResult(category);
    }

    public Task<PostResponseDTO[]> GetPostsByCategoryAsync(string categorySlug)
    {
        Post[] posts = _postRepository
                    .GetPostsByCategory(categorySlug)
                    .Where(post => post.Status == PostStatus.Published)
                    .ToArray();

        if (posts == null || posts.Length == 0)
        {
            throw new CategoryNotFoundException(categorySlug);
        }

        return Task.FromResult(
            posts.Select(
                post => post.MapToPostResponseDTO(_userRepository)
            ).ToArray()
        );
    }

    public Task<CategoryDTO[]> GetAllCategoriesAsync()
    {
        Category[] categories = _categoryRepository.GetAllCategories();
        return Task.FromResult(
            categories.Select(c => c.MapToCategoryDTO()).ToArray()
        );
    }
}