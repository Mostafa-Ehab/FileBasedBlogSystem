using BlogSystem.Features.Posts.GetPost.DTOs;

namespace BlogSystem.Features.Posts.Get
{
    public interface IGetPostHandler
    {
        // Public Routes
        Task<PublicPostDTO> GetPostAsync(string slug);
        Task<PublicPostDTO[]> GetPublicPostsAsync(int pageNumber, int pageSize);
        Task<PublicPostDTO[]> SearchPostsAsync(string searchTerm);

        // Editor Routes
        Task<ManagedPostDTO[]> GetEditorPostsAsync(int pageNumber, int pageSize);
        Task<ManagedPostDTO[]> GetAuthorPostsAsync(string authorId, int pageNumber, int pageSize);
    }
}
