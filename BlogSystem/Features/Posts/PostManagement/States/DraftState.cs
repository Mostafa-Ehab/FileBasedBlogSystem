using BlogSystem.Domain.Entities;
using BlogSystem.Features.Categories.Data;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Tags.Data;
using BlogSystem.Infrastructure.ImageService;
using BlogSystem.Shared.Exceptions;
using BlogSystem.Shared.Exceptions.Categories;
using BlogSystem.Shared.Exceptions.Tags;
using BlogSystem.Shared.Helpers;

namespace BlogSystem.Features.Posts.PostManagement.States;

public class DraftState
{
    private readonly IPostRepository _postRepository;
    private readonly ITagRepository _tagRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly PostBannerImageProvider _bannerImageProvider;
    private readonly PostContentImageProvider _contentImageProvider;

    public DraftState(
        IPostRepository postRepository,
        ITagRepository tagRepository,
        ICategoryRepository categoryRepository,
        PostBannerImageProvider bannerImageProvider,
        PostContentImageProvider contentImageProvider
    )
    {
        _postRepository = postRepository;
        _tagRepository = tagRepository;
        _categoryRepository = categoryRepository;
        _bannerImageProvider = bannerImageProvider;
        _contentImageProvider = contentImageProvider;
    }

    public async Task ValidateAndCreatePost(Post post, IFormFile? image)
    {
        ValidatePostData(post);
        await ValidateAndSaveImage(image, post);
        _postRepository.CreatePost(post);
    }

    public async Task ValidateAndUpdatePost(Post post, IFormFile? image)
    {
        ValidatePostData(post);
        await ValidateAndSaveImage(image, post);
        _postRepository.UpdatePost(post);
    }

    public async Task<string> UploadContentImageAsync(IFormFile file, string userId)
    {
        if (file == null || file.Length == 0)
        {
            throw new ValidationErrorException("Image file is required.");
        }

        return await _contentImageProvider.SaveImageAsync(file);
    }

    private void ValidatePostData(Post post)
    {
        if (!string.IsNullOrWhiteSpace(post.Category) && !_categoryRepository.CategoryExists(post.Category))
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
        if (image != null && image.Length > 0)
        {
            post.ImageUrl = await SavePostBannerImageAsync(image, post.Id);
        }
    }

    private async Task<string> SavePostBannerImageAsync(IFormFile image, string postId)
    {
        if (ImageHelper.IsValidImage(image) == false)
        {
            throw new ValidationErrorException("Invalid image format. Only JPEG, PNG, and GIF are allowed.");
        }

        return await _bannerImageProvider.SaveImageAsync(image, postId);
    }

    private async Task<string> SavePostContentImageAsync(IFormFile image)
    {
        if (ImageHelper.IsValidImage(image) == false)
        {
            throw new ValidationErrorException("Invalid image format. Only JPEG, PNG, and GIF are allowed.");
        }

        return await _contentImageProvider.SaveImageAsync(image);
    }
}