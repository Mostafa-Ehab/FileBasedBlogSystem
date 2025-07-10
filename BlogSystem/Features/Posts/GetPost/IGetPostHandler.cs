using BlogSystem.Features.Posts.GetPost.DTOs;

namespace BlogSystem.Features.Posts.Get
{
    public interface IGetPostHandler
    {
        // Public Routes
        Task<PublicPostDTO> GetPostAsync(string slug);
        Task<PublicPostDTO[]> GetPublicPostsAsync(string? query = null);

        // Editor Routes
        Task<ManagedPostDTO[]> GetManagedPostsAsync(string userId);
    }
}
