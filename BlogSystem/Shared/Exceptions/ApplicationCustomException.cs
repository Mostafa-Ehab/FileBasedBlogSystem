namespace BlogSystem.Shared.Exceptions
{
    public abstract class ApplicationCustomException : Exception
    {
        public int StatusCode { get; }
        public int ErrorCode { get; }
        protected ApplicationCustomException(string message, int statusCode, int errorCode) : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}