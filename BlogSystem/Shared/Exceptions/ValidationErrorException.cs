namespace BlogSystem.Shared.Exceptions
{
    public class ValidationErrorException : ApplicationCustomException
    {
        public ValidationErrorException(string message, int statusCode = 400, int errorCode = 400)
            : base(message, statusCode, errorCode)
        {
        }
        public ValidationErrorException(string message, int errorCode) : base(message, 400, errorCode)
        {
        }
    }
}