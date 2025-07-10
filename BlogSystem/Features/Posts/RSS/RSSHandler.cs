using System.ServiceModel.Syndication;
using System.Xml;
using BlogSystem.Domain.Enums;
using BlogSystem.Features.Posts.Data;

namespace BlogSystem.Features.Posts.RSS
{
    public class RSSHandler : IRSSHandler
    {
        private readonly IPostRepository _postRepository;

        public RSSHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public Task<string> GenerateRSSFeedAsync()
        {
            var posts = _postRepository.GetAllPosts()
                .Where(p => p.Status == PostStatus.Published)
                .OrderByDescending(p => p.UpdatedAt)
                .Take(20);

            if (!posts.Any())
            {
                return Task.FromResult(string.Empty);
            }

            var feed = new SyndicationFeed(
                "Blog RSS Feed",
                "Latest posts from the blog",
                new Uri("https://example.com"),
                posts.Last().UpdatedAt.ToString("o"),
                new DateTimeOffset(posts.First().UpdatedAt),
                posts.Select(p => new SyndicationItem(
                    p.Title,
                    p.Description,
                    new Uri($"https://example.com/posts?id={p.Slug}"),
                    p.Id,
                    new DateTimeOffset(p.UpdatedAt)
                ))
            );

            using var writer = new StringWriter();
            using (var xmlWriter = XmlWriter.Create(writer))
            {
                var rssFormatter = new Rss20FeedFormatter(feed);
                rssFormatter.WriteTo(xmlWriter);
                xmlWriter.Flush();
                return Task.FromResult(writer.ToString());
            }
        }
    }
}
