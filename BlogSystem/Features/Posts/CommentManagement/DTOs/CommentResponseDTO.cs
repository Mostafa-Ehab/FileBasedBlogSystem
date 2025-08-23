namespace BlogSystem.Features.Posts.CommentManagement.DTOs;

public class CommentResponseDTO
{
    public string Id { get; set; } = string.Empty;
    public string PostId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public CommentAuthorDTO User { get; set; } = new();
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
