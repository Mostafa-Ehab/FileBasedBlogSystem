namespace BlogSystem.Features.Users.Authentication.DTOs;

public record LoginRequestDTO
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}