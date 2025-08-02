using BlogSystem.Features.Posts.GetPost.DTOs;
using BlogSystem.Features.Users.GetUser.DTOs;

namespace BlogSystem.Features.Posts.GetPost;

public interface IGetPostHandler
{
    // Public Routes
    Task<PublicPostDTO> GetPostAsync(string slug);
    Task<PublicPostDTO[]> GetPublicPostsAsync(string? query = null, int page = 1, int pageSize = 10);

    // Editor Routes
    Task<ManagedPostDTO> GetManagedPostAsync(string postId, string userId);
    Task<ManagedPostDTO[]> GetManagedPostsAsync(string userId);
    Task<GetUserDTO[]> GetPostEditorsAsync(string postId, string userId);
}
