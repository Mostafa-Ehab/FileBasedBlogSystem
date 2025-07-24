using BlogSystem.Domain.Entities;
using BlogSystem.Features.Posts.GetPost.DTOs;
using BlogSystem.Features.Posts.PostManagement.DTOs;
using BlogSystem.Features.Users.Data;

namespace BlogSystem.Shared.Mappings;

public static class PostMappingProfile
{
    public static PublicPostDTO MapToPublicPostDTO(this Post post, IUserRepository userRepository)
    {
        return new PublicPostDTO
        {
            Id = post.Id,
            Title = post.Title,
            Description = post.Description,
            Content = post.Content,
            AuthorId = post.AuthorId,
            Author = userRepository.GetUserById(post.AuthorId)?.MapToPostAuthorDTO(),
            Category = post.Category,
            ImageUrl = post.ImageUrl,
            Slug = post.Slug,
            PublishedAt = post.PublishedAt ?? DateTime.UtcNow,
            Tags = post.Tags ?? []
        };
    }

    public static ManagedPostDTO MapToManagedPostDTO(this Post post, IUserRepository userRepository)
    {
        return new ManagedPostDTO
        {
            Id = post.Id,
            Title = post.Title,
            Description = post.Description,
            Content = post.Content,
            AuthorId = post.AuthorId,
            Author = userRepository.GetUserById(post.AuthorId)?.MapToPostAuthorDTO(),
            Category = post.Category,
            ImageUrl = post.ImageUrl,
            Slug = post.Slug,
            CreatedAt = post.CreatedAt,
            Status = post.Status,
            ScheduledAt = post.ScheduledAt,
            PublishedAt = post.PublishedAt,
            Tags = post.Tags ?? []
        };
    }

    public static PostAuthorDTO MapToPostAuthorDTO(this User user)
    {
        return new PostAuthorDTO
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            ProfilePictureUrl = user.ProfilePictureUrl ?? string.Empty,
            Bio = user.Bio ?? string.Empty,
            SocialLinks = user.SocialLinks?.ToDictionary(kv => kv.Key, kv => kv.Value) ?? new Dictionary<string, string>()
        };
    }

    public static PostResponseDTO MapToPostResponseDTO(this Post post, IUserRepository userRepository)
    {
        return new PostResponseDTO
        {
            Id = post.Id,
            Title = post.Title,
            Description = post.Description,
            Content = post.Content,
            AuthorId = post.AuthorId,
            Author = userRepository.GetUserById(post.AuthorId)?.MapToPostAuthorDTO(),
            Category = post.Category,
            ImageUrl = post.ImageUrl,
            Slug = post.Slug,
            CreatedAt = post.CreatedAt,
            Status = post.Status,
            ScheduledAt = post.ScheduledAt,
            PublishedAt = post.PublishedAt,
            Tags = post.Tags ?? []
        };
    }
}
