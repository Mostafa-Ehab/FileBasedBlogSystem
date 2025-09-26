using BlogSystem.Features.Users.Authentication.DTOs;

namespace BlogSystem.Features.Users.Authentication;

public interface IAuthenticationHandler
{
    Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO);
    Task<object> RegisterUserAsync(RegisterUserRequestDTO registerUserRequestDTO);
    Task<LoginResponseDTO> VerifyEmailAsync(VerifyEmailRequestDTO verifyEmailRequestDTO);
}
