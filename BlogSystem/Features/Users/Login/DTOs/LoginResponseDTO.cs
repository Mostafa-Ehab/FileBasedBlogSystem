using BlogSystem.Domain.Enums;

namespace BlogSystem.Features.Users.Login.DTOs;

public record LoginResponseDTO
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Author;
}
