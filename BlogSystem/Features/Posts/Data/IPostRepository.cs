using BlogSystem.Domain.Entities;

namespace BlogSystem.Features.Posts.Data;

public interface IPostRepository
{
    public Post? GetPostById(string id);
    public Post? GetPostBySlug(string slug);
    public Post[] GetPostsByCategory(string categorySlug);
    public Post[] GetPostsByTag(string tagSlug);
    public Post[] GetAllPosts();
    public Post[] GetPublicPosts(int page = 1, int pageSize = 10);
    public Post[] GetAuthorPosts(string authorId);
    public bool PostExists(string id);
    public bool PostSlugExists(string slug, string postId);
    string CreatePost(Post post);
    string UpdatePost(Post post);
    void DeletePost(Post post);
}
