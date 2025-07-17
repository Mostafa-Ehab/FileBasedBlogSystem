using BlogSystem.Domain.Entities;
using System.Text.Json;

namespace BlogSystem.Features.Tags.Data;

public class TagRepository : ITagRepository
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public TagRepository(JsonSerializerOptions jsonSerializerOptions)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public Tag? GetTagBySlug(string slug)
    {
        var path = Path.Combine("Content", "tags", $"{slug}.json");
        if (!File.Exists(path))
        {
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<Tag>(json, _jsonSerializerOptions);
    }

    public Tag[] GetAllTags()
    {
        var path = Path.Combine("Content", "tags");
        if (!Directory.Exists(path))
        {
            return [];
        }

        var files = Directory.GetFiles(path, "*.json");
        return files.Select(file => JsonSerializer.Deserialize<Tag>(File.ReadAllText(file), _jsonSerializerOptions)).Where(tag => tag != null).ToArray()!;
    }

    public Tag CreateTag(Tag tag)
    {
        var path = Path.Combine("Content", "tags", $"{tag.Slug}.json");
        if (File.Exists(path))
        {
            throw new InvalidOperationException($"Tag with slug '{tag.Slug}' already exists.");
        }

        string json = JsonSerializer.Serialize(tag, _jsonSerializerOptions);
        File.WriteAllText(path, json);
        return tag;
    }

    public Tag UpdateTag(Tag tag)
    {
        var path = Path.Combine("Content", "tags", $"{tag.Slug}.json");
        if (!File.Exists(path))
        {
            throw new InvalidOperationException($"Tag with slug '{tag.Slug}' does not exist.");
        }

        string json = JsonSerializer.Serialize(tag, _jsonSerializerOptions);
        File.WriteAllText(path, json);
        return tag;
    }

    public void DeleteTag(Tag tag)
    {
        var existingTag = GetTagBySlug(tag.Slug)!;
        var postsPath = Path.Combine("Content", "posts");
        foreach (var postId in existingTag.Posts)
        {
            var postFilePath = Path.Combine(postsPath, postId, "meta.json");
            if (File.Exists(postFilePath))
            {
                var postJson = File.ReadAllText(postFilePath);
                var postData = JsonSerializer.Deserialize<Post>(postJson, _jsonSerializerOptions);
                if (postData != null)
                {
                    postData.Tags.Remove(existingTag.Slug);
                    var updatedPostJson = JsonSerializer.Serialize(postData, _jsonSerializerOptions);
                    File.WriteAllText(postFilePath, updatedPostJson);
                }
            }
        }

        File.Delete(
            Path.Combine("Content", "tags", $"{tag.Slug}.json")
        );
    }

    public bool TagExists(string slug)
    {
        var path = Path.Combine("Content", "tags", $"{slug}.json");
        return File.Exists(path);
    }
}