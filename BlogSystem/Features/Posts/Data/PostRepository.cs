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

        public string CreatePost(Post post, string Content)
        {
            var postPath = Path.Combine("Content", "posts", post.Id);
            if (!Directory.Exists(postPath))
            {
                Directory.CreateDirectory(postPath);
            }

            File.WriteAllText(Path.Combine(postPath, "meta.json"), JsonSerializer.Serialize(post, _jsonSerializerOptions));
            File.WriteAllText(Path.Combine(postPath, "content.md"), Content);

            _slugResolver.AddSlug(post.Slug, post.Id);

            UpdateCategoryFile(post);
            UpdateTagFile(post);

            return post.Id;
        }

        public bool PostExists(string id)
        {
            var path = Path.Combine("Content", "posts", id);
            return Directory.Exists(path) && File.Exists(Path.Combine(path, "meta.json"));
        }

        private void UpdateCategoryFile(Post post)
        {
            if (string.IsNullOrWhiteSpace(post.Category))
            {
                return;
            }

            var categoryPath = Path.Combine("Content", "categories", $"{post.Category}.json");
            string json = File.ReadAllText(categoryPath);
            Category category = JsonSerializer.Deserialize<Category>(json, _jsonSerializerOptions)!;

            if (!category.Posts.Contains(post.Id))
            {
                category.Posts.Add(post.Id);
                File.WriteAllText(categoryPath, JsonSerializer.Serialize(category, _jsonSerializerOptions));
            }
        }

        private void UpdateTagFile(Post post)
        {
            if (post.Tags == null || post.Tags.Count == 0)
            {
                return;
            }

            foreach (var tag in post.Tags)
            {
                var tagPath = Path.Combine("Content", "tags", $"{tag}.json");
                string json = File.ReadAllText(tagPath);
                Tag existingTag = JsonSerializer.Deserialize<Tag>(json, _jsonSerializerOptions)!;

                if (!existingTag.Posts.Contains(post.Id))
                {
                    existingTag.Posts.Add(post.Id);
                }

                File.WriteAllText(tagPath, JsonSerializer.Serialize(existingTag, _jsonSerializerOptions));
            }
        }
    }
}
