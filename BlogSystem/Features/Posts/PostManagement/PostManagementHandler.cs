using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Enums;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Posts.PostManagement.DTOs;
using BlogSystem.Features.Posts.PostManagement.States;
using BlogSystem.Features.Users.Data;
using BlogSystem.Shared.Exceptions;
using BlogSystem.Shared.Exceptions.Posts;
using BlogSystem.Shared.Helpers;
using BlogSystem.Shared.Mappings;

namespace BlogSystem.Features.Posts.PostManagement;

public class PostManagementHandler : IPostManagementHandler
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly DraftState _draftState;
    private readonly ScheduledState _scheduledState;
    private readonly PublishedState _publishedState;

    public PostManagementHandler(
        IPostRepository postRepository,
        IUserRepository userRepository,
        DraftState draftState,
        ScheduledState scheduledState,
        PublishedState publishedState
    )
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
        _draftState = draftState;
        _scheduledState = scheduledState;
        _publishedState = publishedState;
    }

    public async Task<PostResponseDTO> CreatePostAsync(CreatePostRequestDTO request, string userId)
    {
        // Validate the request
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ValidationErrorException("Title cannot be null or empty.");
        }

        if (!string.IsNullOrWhiteSpace(request.Slug) && _postRepository.PostExists(request.Slug))
        {
            throw new PostSlugAlreadyExistException(request.Slug);
        }

        // Prepare the post data
        var postId = SlugHelper.GenerateUniqueSlug(request.Title, _postRepository.PostExists);
        var post = new Post
        {
            Id = postId,
            Title = request.Title,
            Description = request.Description ?? string.Empty,
            Content = request.Content ?? string.Empty,
            AuthorId = userId,
            Category = request.Category ?? string.Empty,
            Slug = string.IsNullOrWhiteSpace(request.Slug) ? postId : request.Slug,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Status = request.Status,
            ScheduledAt = request.ScheduledAt,
            Tags = (request.Tags ?? []).Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Select(tag => tag.Trim())
                .ToList()
        };

        // Validate the post based on its status
        if (post.Status == PostStatus.Draft)
        {
            await _draftState.ValidateAndCreatePost(post, request.Image);
        }
        else if (post.Status == PostStatus.Scheduled)
        {
            await _scheduledState.ValidateAndCreatePost(post, request.Image);
        }
        else if (post.Status == PostStatus.Published)
        {
            await _publishedState.ValidateAndCreatePost(post, request.Image);
        }

        return post.MapToPostResponseDTO(_userRepository);
    }

    public async Task<PostResponseDTO> UpdatePostAsync(string postId, UpdatePostRequestDTO request, string userId)
    {
        // Fetch the existing post
        var post = _postRepository.GetPostById(postId);
        if (post == null)
        {
            throw new PostNotFoundException(postId);
        }

        // Validate the user permissions
        var user = _userRepository.GetUserById(userId)!;
        if (!user.Posts.Contains(post.Id))
        {
            throw new UnauthorizedAccessException("You do not have permission to edit this post.");
        }

        // Validate the request
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ValidationErrorException("Title cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(request.Slug))
        {
            throw new ValidationErrorException("Slug cannot be null or empty.");
        }

        if (post.Slug != request.Slug && _postRepository.PostSlugExists(request.Slug, postId))
        {
            throw new PostSlugAlreadyExistException(request.Slug);
        }

        var previousStatus = post.Status;
        var previousScheduledAt = post.ScheduledAt;

        // Update the post data
        post.Title = request.Title;
        post.Description = request.Description ?? string.Empty;
        post.Content = request.Content ?? string.Empty;
        post.Category = request.Category ?? string.Empty;
        post.Slug = request.Slug;
        post.UpdatedAt = DateTime.UtcNow;
        post.Status = request.Status;

        // Update tags
        post.Tags = (request.Tags ?? []).Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim())
            .ToList();

        // Validate the updated post based on its status
        if (post.Status == PostStatus.Draft)
        {
            if (previousStatus == PostStatus.Scheduled)
                _scheduledState.UnschedulePost(post);

            await _draftState.ValidateAndUpdatePost(post, request.Image);
        }
        else if (post.Status == PostStatus.Scheduled)
        {
            if (previousStatus == PostStatus.Scheduled && previousScheduledAt != post.ScheduledAt)
                _scheduledState.UnschedulePost(post);

            post.ScheduledAt = request.ScheduledAt;
            await _scheduledState.ValidateAndUpdatePost(post, request.Image);
        }
        else if (post.Status == PostStatus.Published)
        {
            if (previousStatus == PostStatus.Scheduled)
                _scheduledState.UnschedulePost(post);

            await _publishedState.ValidateAndUpdatePost(post, previousStatus, request.Image);
        }

        return post.MapToPostResponseDTO(_userRepository);
    }

    public async Task DeletePostAsync(string postId, string userId)
    {
        // Fetch the existing post
        var post = _postRepository.GetPostById(postId);
        if (post == null)
        {
            throw new PostNotFoundException(postId);
        }

        // Validate the user permissions
        var user = _userRepository.GetUserById(userId)!;
        if (user.Role == UserRole.Author && !user.Posts.Contains(post.Id))
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this post.");
        }

        // Validate the user's ownership of the post
        if (post.AuthorId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this post.");
        }

        // Delete the post
        _postRepository.DeletePost(post);
        await Task.FromResult<object>(null!);
    }
}
