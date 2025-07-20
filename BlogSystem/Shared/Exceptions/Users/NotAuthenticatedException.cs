namespace BlogSystem.Shared.Exceptions.Users;

public class NotAuthenticatedException : ApplicationCustomException
{
    public NotAuthenticatedException(string message) : base(message, 403, 403)
    {
    }
    public NotAuthenticatedException(string message, int errorCode) : base(message, 403, errorCode)
    {
    }
}