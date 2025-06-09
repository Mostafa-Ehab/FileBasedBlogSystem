using BlogSystem.Domain.Models;
using System.Text.Json;

namespace BlogSystem.Features.Posts.Data
{
    public class PostRepository : IPostRepository
    {
        private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly SlugResolver slugResolver;
        public PostRepository()
        {
            jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
            slugResolver = new SlugResolver();
        }

        public Post? GetPostById(string id)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Content", "posts", id);
            if (path == null || !Directory.Exists(path))
            {
                return null;
            }

            string json = File.ReadAllText(Path.Combine(path, "meta.json"));
            Post? items = JsonSerializer.Deserialize<Post>(json, jsonSerializerOptions);

            if (items != null)
            {
                items.Content = File.ReadAllText(Path.Combine(path, "content.md"));
            }

            return items;
        }
        public Post? GetPostBySlug(string slug)
        {
            var id = slugResolver.ResolveSlug(slug);
            return !string.IsNullOrWhiteSpace(id) ? GetPostById(id) : null;
        }
    }
}
