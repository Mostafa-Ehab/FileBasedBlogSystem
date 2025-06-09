namespace BlogSystem.Shared.Exceptions
{
    public abstract class ApplicationException : Exception
    {
        public int StatusCode { get; }
        public int ErrorCode { get; }
        protected ApplicationException(string message, int statusCode, int errorCode): base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}