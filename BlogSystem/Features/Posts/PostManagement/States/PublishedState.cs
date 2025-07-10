
using BlogSystem.Domain.Entities;
using BlogSystem.Features.Categories.Data;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Tags.Data;
using BlogSystem.Infrastructure.ImageService;
using BlogSystem.Shared.Exceptions;
using BlogSystem.Shared.Exceptions.Categories;
using BlogSystem.Shared.Exceptions.Tags;

namespace BlogSystem.Features.Posts.PostManagement.States;

public class PublishedState
{
    private readonly IPostRepository _postRepository;
    private readonly ITagRepository _tagRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly PostImageProvider _imageProvider;
    public PublishedState(
        IPostRepository postRepository,
        ITagRepository tagRepository,
        ICategoryRepository categoryRepository,
        PostImageProvider imageProvider
    )
    {
        _postRepository = postRepository;
        _tagRepository = tagRepository;
        _categoryRepository = categoryRepository;
        _imageProvider = imageProvider;
    }

    public async Task ValidateAndCreatePost(Post post, IFormFile? image)
    {
        if (string.IsNullOrWhiteSpace(post.Description) || string.IsNullOrWhiteSpace(post.Content))
        {
            throw new ValidationErrorException("Post description and content cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(post.Category) || _categoryRepository.CategoryExists(post.Category) == false)
        {
            throw new CategoryNotFoundException(post.Category);
        }

        if (post.Tags == null || post.Tags.Count == 0)
        {
            throw new ValidationErrorException("Post must have at least one tag.");
        }
        foreach (var tag in post.Tags)
        {
            if (!_tagRepository.TagExists(tag))
            {
                throw new TagNotFoundException(tag);
            }
        }

        if (image == null || image.Length == 0)
        {
            throw new ValidationErrorException("Image cannot be null or empty.");
        }

        post.ImageUrl = await SavePostImageAsync(image, post.Id);
        post.PublishedAt = DateTime.UtcNow;
        _postRepository.CreatePost(post);
    }

    private async Task<string> SavePostImageAsync(IFormFile? image, string postId)
    {
        var imagePath = string.Empty;
        if (image != null && image.Length > 0)
        {
            if (_imageProvider.IsValidImage(image) == false)
            {
                throw new ValidationErrorException("Invalid image format. Only JPEG, PNG, and GIF are allowed.");
            }

            var imageUrl = await _imageProvider.SaveImageAsync(image, postId);
            imagePath = $"/images/posts/{postId}/{imageUrl}";
        }

        return imagePath;
    }
}
