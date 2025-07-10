using BlogSystem.Domain.Entities;
using BlogSystem.Features.Categories.Data;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Tags.Data;
using BlogSystem.Infrastructure.ImageService;
using BlogSystem.Infrastructure.Scheduling;
using BlogSystem.Shared.Exceptions;
using BlogSystem.Shared.Exceptions.Categories;
using BlogSystem.Shared.Exceptions.Tags;
using System.Threading.Tasks;

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

        Console.WriteLine($"ScheduledAt: {post.ScheduledAt}");
        Console.WriteLine($"Current Time: {DateTimeOffset.UtcNow}");

        if (post.ScheduledAt == null || post.ScheduledAt <= DateTimeOffset.UtcNow)
        {
            throw new ValidationErrorException("Scheduled time must be in the future.");
        }

        if (image == null || image.Length == 0)
        {
            throw new ValidationErrorException("Image cannot be null or empty.");
        }
        post.ImageUrl = await SavePostImageAsync(image, post.Id);

        post.ScheduleToken = _postScheduler.ScheduleJob<PublishPostService>(
            service => service.RunTask(post.Id),
            post.ScheduledAt.Value
        );
        post.ScheduledAt = post.ScheduledAt.Value;
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
