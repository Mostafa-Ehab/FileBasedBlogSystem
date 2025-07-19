using BlogSystem.Features.Categories.Data;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Shared.Exceptions.Categories;

namespace BlogSystem.Features.Categories.DeleteCategory;

public class DeleteCategoryHandler : IDeleteCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPostRepository _postRepository;

    public DeleteCategoryHandler(ICategoryRepository categoryRepository, IPostRepository postRepository)
    {
        _categoryRepository = categoryRepository;
        _postRepository = postRepository;
    }

    public async Task DeleteCategoryAsync(string slug)
    {
        var category = _categoryRepository.GetCategoryBySlug(slug);
        if (category == null)
        {
            throw new CategoryNotFoundException(slug);
        }

        category.Posts.ForEach(postId =>
        {
            var post = _postRepository.GetPostById(postId);
            _postRepository.DeletePost(post!);
        });

        category.Posts.Clear();

        _categoryRepository.DeleteCategory(category);
        await Task.CompletedTask;
    }
}
