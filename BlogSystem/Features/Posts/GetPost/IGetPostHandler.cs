using BlogSystem.Features.Posts.GetPost.DTOs;

namespace BlogSystem.Features.Posts.Get
{
    public interface IGetPostHandler
    {
        // Public Routes
        Task<PublicPostDTO> GetPostAsync(string slug);
        Task<PublicPostDTO[]> GetPublicPostsAsync();
        Task<PublicPostDTO[]> SearchPostsAsync(string searchTerm);

        // Editor Routes
        Task<ManagedPostDTO[]> GetEditorPostsAsync();
        Task<ManagedPostDTO[]> GetAuthorPostsAsync(string authorId);
    }
}
