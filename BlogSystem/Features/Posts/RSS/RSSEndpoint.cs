namespace BlogSystem.Features.Posts.RSS;

public static class RSSEndpoint
{
    public static void MapRSSEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/rss", async (IRSSHandler handler) =>
        {
            var rssFeed = await handler.GenerateRSSFeedAsync();
            if (string.IsNullOrEmpty(rssFeed))
            {
                return Results.NotFound("RSS feed could not be generated.");
            }
            return Results.Content(rssFeed, "application/rss+xml");
        })
        .WithName("GetRSSFeed")
        .WithTags("RSS");
    }
}
