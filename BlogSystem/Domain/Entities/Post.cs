using BlogSystem.Domain.Enums;

namespace BlogSystem.Domain.Entities;

public class Post
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public PostStatus Status { get; set; } = PostStatus.Draft;
    public DateTime? ScheduledAt { get; set; }
    public string? ScheduleToken { get; set; }
    public DateTime? PublishedAt { get; set; }
    public List<string> Tags { get; set; } = [];
    public List<string> Editors { get; set; } = [];
}
