using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Enums;
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
                .ToArray();
        }

        public Post[] GetPublicPosts(int page = 1, int pageSize = 10)
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
                .Select(file => JsonSerializer.Deserialize<Post>(File.ReadAllText(file), _jsonSerializerOptions))
                .Where(post => post != null && post.Status == PostStatus.Published)
                .Select(post =>
                {
                    post!.Content = File.ReadAllText(Path.Combine(path, post.Id, "content.md"));
                    return post;
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToArray();
        }

        public Post[] GetAuthorPosts(string authorId, int page = 1, int pageSize = 10)
        {
            var path = Path.Combine("Content", "users", authorId, "profile.json");
            if (!File.Exists(path))
            {
                return [];
            }

            string json = File.ReadAllText(path);
            User? user = JsonSerializer.Deserialize<User>(json, _jsonSerializerOptions);
            if (user == null || user.Posts == null || user.Posts.Length == 0)
            {
                return [];
            }

            return user.Posts
                .Select(GetPostById)
                .Where(post => post != null)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToArray()!;
        }

        public string CreatePost(Post post)
        {
            var postPath = Path.Combine("Content", "posts", post.Id);
            if (!Directory.Exists(postPath))
            {
                Directory.CreateDirectory(postPath);
            }

            var content = post.Content;
            post.Content = null!;

            File.WriteAllText(Path.Combine(postPath, "meta.json"), JsonSerializer.Serialize(post, _jsonSerializerOptions));
            File.WriteAllText(Path.Combine(postPath, "content.md"), content);

            _slugResolver.AddSlug(post.Slug, post.Id);

            UpdateCategoryFile(post);
            UpdateTagFile(post);
            UpdateUserFile(post.AuthorId, post.Id);

            return post.Id;
        }

        public string UpdatePost(Post post)
        {
            var existingPost = GetPostById(post.Id);
            if (existingPost == null)
            {
                throw new FileNotFoundException($"Post with ID '{post.Id}' does not exist.");
            }

            var postPath = Path.Combine("Content", "posts", post.Id);
            if (!Directory.Exists(postPath))
            {
                throw new DirectoryNotFoundException($"Post directory '{postPath}' does not exist.");
            }

            var content = post.Content;
            post.Content = null!;
            post.UpdatedAt = DateTime.UtcNow;

            File.WriteAllText(Path.Combine(postPath, "meta.json"), JsonSerializer.Serialize(post, _jsonSerializerOptions));
            File.WriteAllText(Path.Combine(postPath, "content.md"), content);

            _slugResolver.UpdateSlug(existingPost.Slug, post.Slug, post.Id);

            UpdateCategoryFile(existingPost, post);
            UpdateTagFile(existingPost, post);

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

        private void UpdateCategoryFile(Post oldPost, Post newPost)
        {
            if (oldPost.Category == newPost.Category)
            {
                return;
            }

            // Remove from old category
            if (!string.IsNullOrWhiteSpace(oldPost.Category))
            {
                var oldCategoryPath = Path.Combine("Content", "categories", $"{oldPost.Category}.json");
                string oldJson = File.ReadAllText(oldCategoryPath);
                Category oldCategory = JsonSerializer.Deserialize<Category>(oldJson, _jsonSerializerOptions)!;

                if (oldCategory.Posts.Contains(oldPost.Id))
                {
                    oldCategory.Posts.Remove(oldPost.Id);
                    File.WriteAllText(oldCategoryPath, JsonSerializer.Serialize(oldCategory, _jsonSerializerOptions));
                }
            }

            // Add to new category
            if (!string.IsNullOrWhiteSpace(newPost.Category))
            {
                var newCategoryPath = Path.Combine("Content", "categories", $"{newPost.Category}.json");
                string newJson = File.ReadAllText(newCategoryPath);
                Category newCategory = JsonSerializer.Deserialize<Category>(newJson, _jsonSerializerOptions)!;

                if (!newCategory.Posts.Contains(newPost.Id))
                {
                    newCategory.Posts.Add(newPost.Id);
                    File.WriteAllText(newCategoryPath, JsonSerializer.Serialize(newCategory, _jsonSerializerOptions));
                }
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

        private void UpdateTagFile(Post oldPost, Post newPost)
        {
            if (oldPost.Tags == null || oldPost.Tags.Count == 0)
            {
                return;
            }

            // Remove from old tags
            foreach (var tag in oldPost.Tags)
            {
                var tagPath = Path.Combine("Content", "tags", $"{tag}.json");
                if (!File.Exists(tagPath))
                {
                    continue;
                }

                string json = File.ReadAllText(tagPath);
                Tag existingTag = JsonSerializer.Deserialize<Tag>(json, _jsonSerializerOptions)!;

                if (existingTag.Posts.Contains(oldPost.Id))
                {
                    existingTag.Posts.Remove(oldPost.Id);
                    File.WriteAllText(tagPath, JsonSerializer.Serialize(existingTag, _jsonSerializerOptions));
                }
            }

            // Add to new tags
            if (newPost.Tags != null && newPost.Tags.Count > 0)
            {
                foreach (var tag in newPost.Tags)
                {
                    var tagPath = Path.Combine("Content", "tags", $"{tag}.json");
                    string json = File.ReadAllText(tagPath);
                    Tag existingTag = JsonSerializer.Deserialize<Tag>(json, _jsonSerializerOptions)!;

                    if (!existingTag.Posts.Contains(newPost.Id))
                    {
                        existingTag.Posts.Add(newPost.Id);
                        File.WriteAllText(tagPath, JsonSerializer.Serialize(existingTag, _jsonSerializerOptions));
                    }
                }
            }
        }

        private void UpdateUserFile(string userId, string postId)
        {
            var userPath = Path.Combine("Content", "users", userId, "profile.json");
            if (!File.Exists(userPath))
            {
                return;
            }

            string json = File.ReadAllText(userPath);
            User user = JsonSerializer.Deserialize<User>(json, _jsonSerializerOptions)!;

            if (!user.Posts.Contains(postId))
            {
                user.Posts = [.. user.Posts, postId];
                File.WriteAllText(userPath, JsonSerializer.Serialize(user, _jsonSerializerOptions));
            }
        }
    }
}
