namespace BlogSystem.Shared.Exceptions.Users;

public class EmailAlreadyExistException : ApplicationCustomException
{
    public EmailAlreadyExistException(string email) : base($"User with email '{email}' already exists.", 409, 409)
    {
    }
}