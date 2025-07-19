using BlogSystem.Domain.Entities;
using System.Text.Json;

namespace BlogSystem.Features.Categories.Data;

public class CategoryRepository : ICategoryRepository
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public CategoryRepository(JsonSerializerOptions jsonSerializerOptions)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public Category? GetCategoryBySlug(string slug)
    {
        var path = Path.Combine("Content", "categories", $"{slug}.json");
        if (!File.Exists(path))
        {
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<Category>(json, _jsonSerializerOptions);
    }

    public Category[] GetAllCategories()
    {
        var path = Path.Combine("Content", "categories");
        if (!Directory.Exists(path))
        {
            return [];
        }

        var files = Directory.GetFiles(path, "*.json");
        return files
            .Select(file => JsonSerializer.Deserialize<Category>(
                File.ReadAllText(file), _jsonSerializerOptions
            ))
            .Where(category => category != null)
            .ToArray()!;
    }

    public Category CreateCategory(Category category)
    {
        var path = Path.Combine("Content", "categories", $"{category.Slug}.json");
        if (File.Exists(path))
        {
            throw new InvalidOperationException($"Category with slug '{category.Slug}' already exists.");
        }

        string json = JsonSerializer.Serialize(category, _jsonSerializerOptions);
        File.WriteAllText(path, json);
        return category;
    }

    public Category UpdateCategory(Category category)
    {
        var path = Path.Combine("Content", "categories", $"{category.Slug}.json");
        if (!File.Exists(path))
        {
            throw new InvalidOperationException($"Category with slug '{category.Slug}' does not exist.");
        }

        string json = JsonSerializer.Serialize(category, _jsonSerializerOptions);
        File.WriteAllText(path, json);
        return category;
    }

    public void DeleteCategory(Category category)
    {
        if (category.Posts != null && category.Posts.Count > 0)
        {
            throw new InvalidOperationException($"Cannot delete category '{category.Slug}' because it has associated posts.");
        }

        File.Delete(
            Path.Combine("Content", "categories", $"{category.Slug}.json")
        );
    }

    public bool CategoryExists(string slug)
    {
        var path = Path.Combine("Content", "categories", $"{slug}.json");
        return File.Exists(path);
    }
}
