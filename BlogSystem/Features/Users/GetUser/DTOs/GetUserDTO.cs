using BlogSystem.Domain.Enums;

namespace BlogSystem.Features.Users.GetUser.DTOs;

public class GetUserDTO
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Author;
    public DateTime CreatedAt { get; set; }
    public string[] Posts { get; set; } = [];
    public Dictionary<string, string> SocialLinks { get; set; } = [];
}
