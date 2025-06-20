using BlogSystem.Domain.Entities;
using BlogSystem.Features.Posts.GetPost.DTOs;

namespace BlogSystem.Features.Posts.Get
{
    public interface IGetPostHandler
    {
        Task<GetPostDTO> GetPostAsync(string slug);
        Task<GetPostDTO[]> GetAllPostsAsync(int pageNumber, int pageSize);
        Task<GetPostDTO[]> SearchPostsAsync(string searchTerm);
    }
}
