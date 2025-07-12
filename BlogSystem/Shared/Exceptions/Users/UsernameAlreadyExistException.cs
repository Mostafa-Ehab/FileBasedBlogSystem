namespace BlogSystem.Shared.Exceptions.Users;

public class UsernameAlreadyExistException : ApplicationCustomException
{
    public UsernameAlreadyExistException(string username) : base($"User with username '{username}' already exists.", 409, 409)
    {
    }
}