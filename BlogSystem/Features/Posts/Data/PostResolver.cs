using BlogSystem.Domain.Enums;
using System.Text.Json;

namespace BlogSystem.Features.Posts.Data;

public class PostResolver
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private Dictionary<string, string> _slugCache = [];
    private Dictionary<string, PostStatus> _statusCache = [];

    public PostResolver(JsonSerializerOptions jsonSerializerOptions)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
        LoadPostsData();
    }

    public string? ResolveSlug(string slug)
    {
        return _slugCache.TryGetValue(slug.ToLowerInvariant(), out var cachedId) ? cachedId : null;
    }

    public PostStatus GetStatusById(string id)
    {
        return _statusCache[id];
    }

    public void AddPost(string slug, string id, PostStatus status)
    {
        if (string.IsNullOrWhiteSpace(slug) || string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Slug and id cannot be null or empty.");
        }

        _slugCache[slug.ToLowerInvariant()] = id;
        _statusCache[id] = status;
        SavePostsData();
    }

    public void RemoveSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException("Slug cannot be null or empty.");
        }

        _statusCache.Remove(_slugCache[slug.ToLowerInvariant()]);
        _slugCache.Remove(slug.ToLowerInvariant());
        SavePostsData();
    }

    public void UpdatePost(string oldSlug, string newSlug, string id, PostStatus status)
    {
        if (string.IsNullOrWhiteSpace(oldSlug) || string.IsNullOrWhiteSpace(newSlug) || string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Old slug, new slug, and id cannot be null or empty.");
        }

        RemoveSlug(oldSlug);
        AddPost(newSlug, id, status);
    }

    public bool SlugExists(string slug)
    {
        return _slugCache.ContainsKey(slug.ToLowerInvariant());
    }

    private void LoadPostsData()
    {
        var pathsFile = Path.Combine("Content", "posts", "slugs.json");
        if (File.Exists(pathsFile))
        {
            var json = File.ReadAllText(pathsFile);
            var pathsData = JsonSerializer.Deserialize<PostsJson>(json, _jsonSerializerOptions)
                ?? throw new InvalidOperationException("Failed to load slugs.json");
            _slugCache = pathsData.Posts.ToDictionary(p => p.Slug.ToLowerInvariant(), p => p.Id);
            _statusCache = pathsData.Posts.ToDictionary(p => p.Id, p => p.Status);
        }
        else
        {
            _slugCache = [];
            _statusCache = [];
        }
    }

    private void SavePostsData()
    {
        var pathsFile = Path.Combine("Content", "posts", "slugs.json");
        var pathsData = new PostsJson
        {
            Posts = _slugCache.Select(kvp => new PostData
            {
                Slug = kvp.Key,
                Id = kvp.Value,
                Status = _statusCache[kvp.Value]
            }).ToList()
        };
        var json = JsonSerializer.Serialize(pathsData, _jsonSerializerOptions);
        File.WriteAllText(pathsFile, json);
    }
}

internal class PostsJson
{
    public List<PostData> Posts { get; set; } = [];
}
internal class PostData
{
    public string Slug { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public PostStatus Status { get; set; } = PostStatus.Draft;
}