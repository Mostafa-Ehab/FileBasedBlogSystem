namespace BlogSystem.Features.Posts.CommentManagement.DTOs;

public class CommentAuthorDTO
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
}