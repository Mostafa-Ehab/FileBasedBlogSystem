using BlogSystem.Domain.Entities;

namespace BlogSystem.Features.Posts.Data;

public interface IPostRepository
{
    // Get single post methods
    public Post? GetPostById(string id);
    public Post? GetPostBySlug(string slug);

    // Get public posts methods
    public Post[] GetPublicPosts(int page = 1, int pageSize = 10);
    public Post[] GetPublicPostsByCategory(string categorySlug, int page = 1, int pageSize = 10);
    public Post[] GetPublicPostsByTag(string tagSlug, int page = 1, int pageSize = 10);
    public Post[] GetAuthorPublicPosts(string authorId, int page = 1, int pageSize = 10);

    // Get managed posts methods
    public Post[] GetAllPosts();
    public Post[] GetAuthorPosts(string authorId);

    // Manage Comments
    public Comment[] GetCommentsByPostId(string postId);
    public Comment CreateComment(Comment comment);
    public Comment EditComment(Comment comment);
    public void DeleteComment(Comment comment);

    // Post management methods
    string CreatePost(Post post);
    string UpdatePost(Post post);
    void DeletePost(Post post);

    // Post existence checks
    public bool PostExists(string id);
    public bool PostSlugExists(string slug, string postId);
}
