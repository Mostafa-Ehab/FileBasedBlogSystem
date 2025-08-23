namespace BlogSystem.Shared.Exceptions.Comments;

public class CommentNotFoundException : ApplicationCustomException
{
    public CommentNotFoundException(string id) : base($"Comment '{id}' not found.", 404, 404)
    {
    }
    public CommentNotFoundException(string id, int errorCode) : base($"Comment '{id}' not found.", 404, errorCode)
    {
    }
}