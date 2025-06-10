using BlogSystem.Domain.Models;

namespace BlogSystem.Features.Posts.Data
{
    public interface IPostRepository
    {
        public Post? GetPostById(string id);
        public Post? GetPostBySlug(string slug);
        public Post[] GetPostsByCategory(string categorySlug);
    }
}
