using BlogSystem.Features.Users.Login.DTOs;

namespace BlogSystem.Features.Users.Login
{
    public interface ILoginHandler
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO);
    }
}
