namespace BlogSystem.Features.Posts.RSS
{
    public interface IRSSHandler
    {
        Task<string> GenerateRSSFeedAsync();
    }
}