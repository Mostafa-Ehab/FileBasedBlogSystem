namespace BlogSystem.Features.Posts.Data
{
    using System.IO;
    using System.Text.Json;
    using BlogSystem.Domain.Models;

    public class SlugResolver
    {
        private readonly JsonSerializerOptions jsonSerializerOptions;
        private Dictionary<string, string> slugCache = new();
        public SlugResolver()
        {
            jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
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
        public bool SlugExists(string slug)
        {
            return slugCache.ContainsKey(slug.ToLowerInvariant());
        }

        private void LoadSlugs()
        {
            var pathsFile = Path.Combine(AppContext.BaseDirectory, "Content", "posts", "slugs.json");
            if (File.Exists(pathsFile))
            {
                var json = File.ReadAllText(pathsFile);
                var pathsData = JsonSerializer.Deserialize<PathsJson>(json, jsonSerializerOptions)
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
            var pathsFile = Path.Combine(AppContext.BaseDirectory, "Content", "posts", "slugs.json");
            var pathsData = new PathsJson
            {
                Posts = slugCache.Select(kvp => new PostPath { Slug = kvp.Key, Id = kvp.Value }).ToList()
            };
            var json = JsonSerializer.Serialize(pathsData, jsonSerializerOptions);
            File.WriteAllText(pathsFile, json);
        }

        private class PathsJson
        {
            public List<PostPath> Posts { get; set; } = new();
        }
        private class PostPath
        {
            public string Slug { get; set; } = string.Empty;
            public string Id { get; set; } = string.Empty;
        }
    }
}