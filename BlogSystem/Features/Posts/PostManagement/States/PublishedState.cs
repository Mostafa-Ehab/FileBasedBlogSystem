
using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Enums;
using BlogSystem.Features.Categories.Data;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Tags.Data;
using BlogSystem.Infrastructure.ImageService;
using BlogSystem.Shared.Exceptions;
using BlogSystem.Shared.Exceptions.Categories;
using BlogSystem.Shared.Exceptions.Tags;
using BlogSystem.Shared.Helpers;

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
        ValidatePostData(post);
        await ValidateAndSaveImage(image, post);
        post.PublishedAt = DateTime.UtcNow;
        _postRepository.CreatePost(post);
    }

    public async Task ValidateAndUpdatePost(Post post, PostStatus previousStatus, IFormFile? image)
    {
        ValidatePostData(post);
        await ValidateAndSaveImage(image, post);

        if (previousStatus != PostStatus.Published)
            post.PublishedAt = DateTime.UtcNow;

        _postRepository.UpdatePost(post);
    }

    private void ValidatePostData(Post post)
    {
        if (string.IsNullOrWhiteSpace(post.Description) || string.IsNullOrWhiteSpace(post.Content))
        {
            throw new ValidationErrorException("Post description and content cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(post.Category) || _categoryRepository.CategoryExists(post.Category) == false)
        {
            throw new CategoryNotFoundException(post.Category);
        }

        if (post.Tags != null && post.Tags.Count > 0)
        {
            post.Tags.ForEach(tag =>
            {
                var tagSlug = SlugHelper.GenerateSlug(tag);
                if (!_tagRepository.TagExists(tagSlug))
                {
                    _tagRepository.CreateTag(new Tag
                    {
                        Name = tag,
                        Slug = tagSlug,
                        Description = string.Empty,
                        Posts = []
                    });
                }
            });
post.Tags = post.Tags.Select(tag => SlugHelper.GenerateSlug(tag)).ToList();
        }
    }

    private async Task ValidateAndSaveImage(IFormFile? image, Post post)
    {
        if ((image == null || image.Length == 0) && string.IsNullOrWhiteSpace(post.ImageUrl))
        {
            throw new ValidationErrorException("Image cannot be null or empty.");
        }

        post.ImageUrl = string.IsNullOrWhiteSpace(post.ImageUrl) ?
            await SavePostImageAsync(image!, post.Id) : post.ImageUrl;
    }

    private async Task<string> SavePostImageAsync(IFormFile image, string postId)
    {
        if (_imageProvider.IsValidImage(image) == false)
        {
            throw new ValidationErrorException("Invalid image format. Only JPEG, PNG, and GIF are allowed.");
        }

        var imageUrl = await _imageProvider.SaveImageAsync(image, postId);
        return $"/images/posts/{postId}/{imageUrl}";
    }
}
