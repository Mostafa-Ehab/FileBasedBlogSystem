using System.Text.Json;

namespace BlogSystem.Features.Posts.Data;

public class SlugResolver
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private Dictionary<string, string> slugCache = [];

    public SlugResolver(JsonSerializerOptions jsonSerializerOptions)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
        LoadSlugs();
    }

    public string? ResolveSlug(string slug)
    {
        return slugCache.TryGetValue(slug.ToLowerInvariant(), out var cachedId) ? cachedId : null;
    }

    public void AddSlug(string slug, string id)
    {
        if (string.IsNullOrWhiteSpace(slug) || string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Slug and id cannot be null or empty.");
        }

        slugCache[slug.ToLowerInvariant()] = id;
        SaveSlugs();
    }

    public void RemoveSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException("Slug cannot be null or empty.");
        }

        slugCache.Remove(slug.ToLowerInvariant());
        SaveSlugs();
    }

    public void UpdateSlug(string oldSlug, string newSlug, string id)
    {
        if (string.IsNullOrWhiteSpace(oldSlug) || string.IsNullOrWhiteSpace(newSlug) || string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Old slug, new slug, and id cannot be null or empty.");
        }

        RemoveSlug(oldSlug);
        AddSlug(newSlug, id);
    }

    public bool SlugExists(string slug)
    {
        return slugCache.ContainsKey(slug.ToLowerInvariant());
    }

    private void LoadSlugs()
    {
        var pathsFile = Path.Combine("Content", "posts", "slugs.json");
        if (File.Exists(pathsFile))
        {
            var json = File.ReadAllText(pathsFile);
            var pathsData = JsonSerializer.Deserialize<PostsJson>(json, _jsonSerializerOptions)
                ?? throw new InvalidOperationException("Failed to load slugs.json");
            slugCache = pathsData.Posts.ToDictionary(p => p.Slug.ToLowerInvariant(), p => p.Id);
        }
        else
        {
            slugCache = new();
        }
    }

    private void SaveSlugs()
    {
        var pathsFile = Path.Combine("Content", "posts", "slugs.json");
        var pathsData = new PostsJson
        {
            Posts = slugCache.Select(kvp => new PostData { Slug = kvp.Key, Id = kvp.Value }).ToList()
        };
        var json = JsonSerializer.Serialize(pathsData, _jsonSerializerOptions);
        File.WriteAllText(pathsFile, json);
    }
}

internal class PostsJson
{
    public List<PostData> Posts { get; set; } = new();
}
internal class PostData
{
    public string Slug { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
}