using System.Text.Json;
using BlogSystem.Domain.Entities;

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

    public bool TagExists(string slug)
    {
        var path = Path.Combine("Content", "tags", $"{slug}.json");
        return File.Exists(path);
    }
}