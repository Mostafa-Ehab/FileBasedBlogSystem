using BlogSystem.Features.Users.CreateUser.DTOs;

namespace BlogSystem.Features.Users.CreateUser
{
    public interface ICreateUserHandler
    {
        Task<CreatedUserDTO> CreateUserAsync(CreateUserRequestDTO createUserRequestDTO);
    }
}