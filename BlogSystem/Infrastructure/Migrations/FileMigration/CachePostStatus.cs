using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlogSystem.Infrastructure.Migrations.FileMigration;

public class CachePostStatus : IFileMigration
{
    public string Id => "001-cache-post-status";
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        IgnoreReadOnlyProperties = true,
        Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
    };

    public void Up()
    {
        var posts = new PostsJson();
        foreach (var post in Directory.GetDirectories(Path.Combine("Content", "posts")))
        {
            var metaFile = Path.Combine(post, "meta.json");
            if (!File.Exists(metaFile)) continue;

            // Deserialize the post metadata to get the status
            var postJson = JsonSerializer.Deserialize<Post>(
                File.ReadAllText(metaFile), _jsonSerializerOptions
            ) ?? throw new InvalidOperationException($"Failed to load post metadata from {metaFile}");

            posts.Posts.Add(new PostData
            {
                Id = postJson.Id,
                Slug = postJson.Slug,
                Status = postJson.Status
            });
        }
        var pathsFile = Path.Combine("Content", "posts", "slugs.json");
        var json = JsonSerializer.Serialize(posts, _jsonSerializerOptions);
        File.WriteAllText(pathsFile, json);
    }

    private class PostsJson
    {
        public List<PostData> Posts { get; set; } = [];
    }

    private class PostData
    {
        public string Id { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public PostStatus Status { get; set; }
    }
}
