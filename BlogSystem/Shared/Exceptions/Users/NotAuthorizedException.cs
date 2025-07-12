namespace BlogSystem.Shared.Exceptions.Users;

public class NotAuthorizedException : ApplicationCustomException
{
    public NotAuthorizedException(string message) : base(message, 403, 403)
    {
    }
    public NotAuthorizedException(string message, int errorCode) : base(message, 403, errorCode)
    {
    }
}