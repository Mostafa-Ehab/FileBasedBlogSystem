namespace BlogSystem.Features.Users.Authentication.DTOs;

public class VerifyEmailRequestDTO
{
    public string Email { get; set; } = string.Empty;
    public string VerificationCode { get; set; } = string.Empty;
}
