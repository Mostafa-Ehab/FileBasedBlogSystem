using System.Text.Json;
using BlogSystem.Domain.Entities;

namespace BlogSystem.Features.Categories.Data
{
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
    }
}
