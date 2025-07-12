using BlogSystem.Domain.Enums;

namespace BlogSystem.Features.Posts.PostManagement.DTOs;

public class CreatePostRequestDTO
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? Content { get; set; } = string.Empty;
    public string? Category { get; set; } = string.Empty;
    public IFormFile? Image { get; set; }
    public string? Slug { get; set; } = string.Empty;
    public PostStatus Status { get; set; } = PostStatus.Draft;
    public DateTime? ScheduledAt { get; set; }
    public string[] Tags { get; set; } = [];
}