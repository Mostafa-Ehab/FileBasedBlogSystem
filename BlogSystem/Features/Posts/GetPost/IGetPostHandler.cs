using BlogSystem.Features.Posts.GetPost.DTOs;

namespace BlogSystem.Features.Posts.GetPost;

public interface IGetPostHandler
{
    // Public Routes
    Task<PublicPostDTO> GetPostAsync(string slug);
    Task<PublicPostDTO[]> GetPublicPostsAsync(string? query = null);

    // Editor Routes
    Task<ManagedPostDTO> GetManagedPostAsync(string postId, string userId);
    Task<ManagedPostDTO[]> GetManagedPostsAsync(string userId);
}
