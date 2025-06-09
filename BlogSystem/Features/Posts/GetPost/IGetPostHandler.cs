using BlogSystem.Domain.Models;

namespace BlogSystem.Features.Posts.Get
{
    public interface IGetPostHandler
    {
        Task<Post> GetPostAsync(string slug);
    }
}
