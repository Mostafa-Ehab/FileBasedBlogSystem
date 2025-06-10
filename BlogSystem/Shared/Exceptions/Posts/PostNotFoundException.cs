namespace BlogSystem.Shared.Exceptions.Posts
{
    public class PostNotFoundException : ApplicationCustomException
    {
        public PostNotFoundException(string id) : base($"Post '{id}' not found.", 404, 404)
        {
        }
        public PostNotFoundException(string id, int errorCode) : base($"Post '{id}' not found.", 404, errorCode)
        {
        }
    }
}