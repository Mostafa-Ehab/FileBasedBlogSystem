namespace BlogSystem.Shared.Exceptions.Tags
{
    public class TagNotFoundException : ApplicationCustomException
    {
        public TagNotFoundException(string slug) : base($"Tag with slug '{slug}' not found.", 404, 404)
        {
        }
        public TagNotFoundException(string slug, int errorCode) : base($"Tag with slug '{slug}' not found.", 404, errorCode)
        {
        }
    }
}