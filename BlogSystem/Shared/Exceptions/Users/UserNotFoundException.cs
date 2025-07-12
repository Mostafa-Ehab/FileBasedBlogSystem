namespace BlogSystem.Shared.Exceptions.Users;

public class UserNotFoundException : ApplicationCustomException
{
    public UserNotFoundException(string id) : base($"User '{id}' not found.", 404, 404)
    {
    }
    public UserNotFoundException(string id, int errorCode) : base($"User '{id}' not found.", 404, errorCode)
    {
    }
}