using BlogSystem.Domain.Enums;

namespace BlogSystem.Features.Posts.GetPost.DTOs;

public class ManagedPostDTO
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public PostAuthorDTO? Author { get; set; }
    public string Category { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public PostStatus Status { get; set; } = PostStatus.Draft;
    public DateTime? ScheduledAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public List<string> Tags { get; set; } = [];
    public List<string> Editors { get; set; } = [];
}
