namespace BlogSystem.Shared.Exceptions.Users
{
    public class NotAuthenticatedException : ApplicationCustomException
    {
        public NotAuthenticatedException(string message) : base(message, 401, 401)
        {
        }
        public NotAuthenticatedException(string message, int errorCode) : base(message, 401, errorCode)
        {
        }
    }
}