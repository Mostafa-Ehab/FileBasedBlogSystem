namespace BlogSystem.Features.Users.CreateUser.DTOs;

public class RegisterUserRequestDTO
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
}