using BlogSystem.Features.Users.Data;
using BlogSystem.Shared.Exceptions.Users;

namespace BlogSystem.Features.Users.DeleteUser;

public class DeleteUserHandler : IDeleteUserHandler
{
    private readonly IUserRepository _userRepository;

    public DeleteUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task DeleteUserAsync(string userId)
    {
        var user = _userRepository.GetUserById(userId);
        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }
        _userRepository.DeleteUser(user);
    }
}
