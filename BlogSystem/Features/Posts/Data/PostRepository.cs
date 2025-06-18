using BlogSystem.Domain.Entities;
using System.Text.Json;

namespace BlogSystem.Features.Posts.Data
{
    public class PostRepository : IPostRepository
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly SlugResolver _slugResolver;

        public PostRepository(JsonSerializerOptions jsonSerializerOptions, SlugResolver slugResolver)
        {
            _jsonSerializerOptions = jsonSerializerOptions;
            _slugResolver = slugResolver;
        }

        public Post? GetPostById(string id)
        {
            var path = Path.Combine("Content", "posts", id);
            if (path == null || !Directory.Exists(path))
            {
                return null;
            }

            string json = File.ReadAllText(Path.Combine(path, "meta.json"));
            Post? items = JsonSerializer.Deserialize<Post>(json, _jsonSerializerOptions);

            if (items != null)
            {
                items.Content = File.ReadAllText(Path.Combine(path, "content.md"));
            }

            return items;
        }

        public Post? GetPostBySlug(string slug)
        {
            var id = _slugResolver.ResolveSlug(slug);
            return !string.IsNullOrWhiteSpace(id) ? GetPostById(id) : null;
        }

        public Post[] GetPostsByCategory(string categorySlug)
        {
            var path = Path.Combine("Content", "categories", $"{categorySlug}.json");
            if (!File.Exists(path))
            {
                return [];
            }

            string json = File.ReadAllText(path);
            Category? category = JsonSerializer.Deserialize<Category>(json, _jsonSerializerOptions);
            if (category == null || category.Posts == null || category.Posts.Count == 0)
            {
                return [];
            }

            return category.Posts
                .Select(GetPostById)
                .Where(post => post != null)
                .ToArray()!;
        }

        public Post[] GetPostsByTag(string tagSlug)
        {
            var path = Path.Combine("Content", "tags", $"{tagSlug}.json");
            if (!File.Exists(path))
            {
                return [];
            }

            string json = File.ReadAllText(path);
            Tag? tag = JsonSerializer.Deserialize<Tag>(json, _jsonSerializerOptions);
            if (tag == null || tag.Posts == null || tag.Posts.Count == 0)
            {
                return [];
            }

            return tag.Posts
                .Select(GetPostById)
                .Where(post => post != null)
                .ToArray()!;
        }

        public Post[] GetAllPosts(int page = 1, int pageSize = 10)
        {
            var path = Path.Combine("Content", "posts");
            if (!Directory.Exists(path))
            {
                return [];
            }

            var postFiles = Directory.GetDirectories(path)
                .Select(dir => Path.Combine(dir, "meta.json"))
                .Where(File.Exists)
                .OrderByDescending(File.GetLastWriteTime);

            return postFiles
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(file => JsonSerializer.Deserialize<Post>(File.ReadAllText(file), _jsonSerializerOptions))
                .Where(post => post != null)
                .Select(post =>
                {
                    post!.Content = File.ReadAllText(Path.Combine(path, post.Id, "content.md"));
                    return post;
                })
                .ToArray()!;
        }
    }
}
