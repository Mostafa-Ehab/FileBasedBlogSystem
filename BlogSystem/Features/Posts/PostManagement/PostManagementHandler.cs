using System.Security.Claims;
using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Enums;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Posts.PostManagement.DTOs;
using BlogSystem.Features.Posts.PostManagement.States;
using BlogSystem.Features.Users.Data;
using BlogSystem.Infrastructure.ImageService;
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

    public PostManagementHandler(
        IPostRepository postRepository,
        IUserRepository userRepository,
        DraftState draftState,
        ScheduledState scheduledState
    )
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
        _draftState = draftState;
        _scheduledState = scheduledState;
    }

    public async Task<PostResponseDTO> CreatePostAsync(CreatePostRequestDTO request, ClaimsPrincipal user)
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
            AuthorId = user.FindFirstValue("Id")!,
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
            Console.WriteLine($"Post status: {post.Status}");
            await _scheduledState.ValidateAndCreatePost(post, request.Image);
        }
        else if (post.Status == PostStatus.Published)
        {
            throw new NotImplementedException("Published state validation is not implemented yet.");
        }

        return post.MapToPostResponseDTO(_userRepository);
    }
}
