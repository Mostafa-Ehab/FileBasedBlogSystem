using BlogSystem.Features.Users.Data;
using BlogSystem.Features.Users.GetUser.DTOs;
using BlogSystem.Shared.Exceptions.Users;
using BlogSystem.Shared.Mappings;

namespace BlogSystem.Features.Users.GetUser;

public class GetUserHandler : IGetUserHandler
{
    private readonly IUserRepository _userRepository;

    public GetUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<GetUserDTO[]> GetAllUsers()
    {
        var users = _userRepository.GetAllUsers();
        return Task.FromResult(
            users.Select(
                user => user.MapToGetUserDTO()
            ).ToArray()
        );
    }

    public Task<GetMyProfileDTO> GetMyProfile(string userId)
    {
        var user = _userRepository.GetUserById(userId);
        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }
        return Task.FromResult(user.MapToGetMyProfileDTO());
    }
}
