using BlogSystem.Domain.Entities;

namespace BlogSystem.Features.Posts.Data
{
    public interface IPostRepository
    {
        public Post? GetPostById(string id);
        public Post? GetPostBySlug(string slug);
        public Post[] GetPostsByCategory(string categorySlug);
        public Post[] GetPostsByTag(string tagSlug);
        public Post[] GetAllPosts(int page = 1, int pageSize = 10);
        public bool PostExists(string id);
        string CreatePost(Post post, string Content);
    }
}
