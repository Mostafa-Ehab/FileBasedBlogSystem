using BlogSystem.Domain.Entities;
using BlogSystem.Features.Categories.Data;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Shared.Exceptions.Categories;

namespace BlogSystem.Features.Categories.GetCategory
{
    public class GetCategoryHandler : IGetCategoryHandler
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPostRepository _postRepository;

        public GetCategoryHandler(ICategoryRepository categoryRepository, IPostRepository postRepository)
        {
            _categoryRepository = categoryRepository;
            _postRepository = postRepository;
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

        public Task<Post[]> GetPostsByCategoryAsync(string categorySlug)
        {
            Post[] posts = _postRepository.GetPostsByCategory(categorySlug);
            if (posts == null || posts.Length == 0)
            {
                throw new CategoryNotFoundException(categorySlug);
            }
            return Task.FromResult(posts);
        }

        public Task<Category[]> GetAllCategoriesAsync()
        {
            Category[] categories = _categoryRepository.GetAllCategories();
            return Task.FromResult(categories);
        }
    }
}