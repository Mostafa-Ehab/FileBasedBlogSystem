namespace BlogSystem.Features.Users.UpdateUser.DTOs;

public class UpdateProfileInfoRequestDTO
{
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public Dictionary<string, string> SocialLinks { get; set; } = [];
}