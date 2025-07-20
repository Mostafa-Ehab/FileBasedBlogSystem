namespace BlogSystem.Shared.Exceptions.Users;

public class NotAuthorizedException : ApplicationCustomException
{
    public NotAuthorizedException(string message) : base(message, 401, 401)
    {
    }
    public NotAuthorizedException(string message, int errorCode) : base(message, 401, errorCode)
    {
    }
}