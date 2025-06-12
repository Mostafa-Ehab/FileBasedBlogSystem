using BlogSystem.Features.Users.CreateUser.DTOs;

namespace BlogSystem.Features.Users.CreateUser
{
    public interface ICreateUserHandler
    {
        Task<CreateUserResponseDTO> CreateUserAsync(CreateUserRequestDTO createUserRequestDTO);
    }
}