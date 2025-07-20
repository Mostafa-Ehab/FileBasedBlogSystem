using BlogSystem.Domain.Entities;
using BlogSystem.Features.Categories.Data;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Tags.Data;
using BlogSystem.Infrastructure.ImageService;
using BlogSystem.Infrastructure.Scheduling;
using BlogSystem.Shared.Exceptions;
using BlogSystem.Shared.Exceptions.Categories;
using BlogSystem.Shared.Exceptions.Tags;
using BlogSystem.Shared.Helpers;

namespace BlogSystem.Features.Posts.PostManagement.States;

public class ScheduledState
{
    private readonly IPostRepository _postRepository;
    private readonly ITagRepository _tagRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly PostImageProvider _imageProvider;
    private readonly IScheduler _postScheduler;

    public ScheduledState(
        IPostRepository postRepository,
        ITagRepository tagRepository,
        ICategoryRepository categoryRepository,
        PostImageProvider imageProvider,
        IScheduler postScheduler
    )
    {
        _postRepository = postRepository;
        _tagRepository = tagRepository;
        _categoryRepository = categoryRepository;
        _imageProvider = imageProvider;
        _postScheduler = postScheduler;
    }

    public async Task ValidateAndCreatePost(Post post, IFormFile? image)
    {
        ValidatePostData(post);
        ValidateScheduling(post);
        await ValidateAndSaveImage(image, post);
        SchedulePost(post);
        _postRepository.CreatePost(post);
    }

    public async Task ValidateAndUpdatePost(Post post, IFormFile? image)
    {
        ValidatePostData(post);
        ValidateScheduling(post);
        await ValidateAndSaveImage(image, post);
        SchedulePost(post);
        _postRepository.UpdatePost(post);
    }

    public void UnschedulePost(Post post)
    {
        _postScheduler.CancelJob(post.ScheduleToken!);
        post.ScheduledAt = null;
        post.ScheduleToken = null;
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

    private void ValidateScheduling(Post post)
    {
        if (post.ScheduledAt == null || post.ScheduledAt <= DateTimeOffset.UtcNow)
        {
            throw new ValidationErrorException("Scheduled time must be in the future.");
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

    private void SchedulePost(Post post)
    {
        post.ScheduleToken = _postScheduler.ScheduleJob<PublishPostService>(
                service => service.RunTask(post.Id),
                post.ScheduledAt!.Value
            );
        post.ScheduledAt = post.ScheduledAt.Value;
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
