using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Enums;
using System.Text.Json;

namespace BlogSystem.Features.Posts.Data;

public class PostRepository : IPostRepository
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly PostResolver _postResolver;

    public PostRepository(JsonSerializerOptions jsonSerializerOptions, PostResolver postResolver)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
        _postResolver = postResolver;
    }

    #region Get single post methods
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
        var id = _postResolver.ResolveSlug(slug);
        return !string.IsNullOrWhiteSpace(id) ? GetPostById(id) : null;
    }
    #endregion

    #region Get public posts methods
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
            .OrderByDescending(File.GetLastWriteTime)
            .Where(file => _postResolver.GetStatusById(
                Path.GetDirectoryName(file)?.Split(Path.DirectorySeparatorChar).Last() ?? string.Empty
            ) == PostStatus.Published)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return postFiles
            .Select(file => JsonSerializer.Deserialize<Post>(File.ReadAllText(file), _jsonSerializerOptions))
            .Select(post =>
            {
                post!.Content = File.ReadAllText(Path.Combine(path, post.Id, "content.md"));
                return post;
            })
            .ToArray();
    }

    public Post[] GetPublicPostsByCategory(string categorySlug, int page = 1, int pageSize = 10)
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
            .Where(postId => _postResolver.GetStatusById(postId) == PostStatus.Published)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(GetPostById)
            .OrderByDescending(post => post!.UpdatedAt)
            .ToArray()!;
    }

    public Post[] GetPublicPostsByTag(string tagSlug, int page = 1, int pageSize = 10)
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
            .Where(postId => _postResolver.GetStatusById(postId) == PostStatus.Published)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(GetPostById)
            .OrderByDescending(post => post!.UpdatedAt)
            .ToArray()!;
    }

    public Post[] GetAuthorPublicPosts(string authorId, int page = 1, int pageSize = 10)
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
            .Where(postId => _postResolver.GetStatusById(postId) == PostStatus.Published)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(GetPostById)
            .OrderByDescending(post => post!.UpdatedAt)
            .ToArray()!;
    }
    #endregion

    #region Get Managed posts methods
    public Post[] GetAllPosts()
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
            .Where(post => post != null)
            .Select(post =>
            {
                post!.Content = File.ReadAllText(Path.Combine(path, post.Id, "content.md"));
                return post;
            })
            .ToArray();
    }

    public Post[] GetAuthorPosts(string authorId)
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
            .OrderByDescending(post => post!.UpdatedAt)
            .ToArray()!;
    }
    #endregion

    public Comment[] GetCommentsByPostId(string postId)
    {
        var path = Path.Combine("Content", "posts", postId, "comments.json");
        if (!File.Exists(path))
        {
            return [];
        }

        string json = File.ReadAllText(path);
        CommentsJson? comments = JsonSerializer.Deserialize<CommentsJson>(json, _jsonSerializerOptions);
        return comments?.Comments ?? [];
    }

    public Comment CreateComment(Comment comment)
    {
        var comments = GetCommentsByPostId(comment.PostId).ToList();
        comment.CreatedAt = DateTime.UtcNow;
        comment.UpdatedAt = DateTime.UtcNow;
        comments.Add(comment);
        SaveCommentsForPost(comment.PostId, comments);
        return comment;
    }

    public Comment EditComment(Comment comment)
    {
        throw new NotImplementedException();
    }

    public void DeleteComment(Comment comment)
    {
        throw new NotImplementedException();
    }

    #region Post management methods
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

        _postResolver.AddPost(post.Slug, post.Id, post.Status);

        UpdateCategoryFile(post);
        UpdateTagFile(post);
        AddPostToUser(post.AuthorId, post.Id);
        UpdateEditors(post);

        post.Content = content; // Restore content for return
        return post.Id;
    }

    public string UpdatePost(Post post)
    {
        var existingPost = GetPostById(post.Id)!;
        var postPath = Path.Combine("Content", "posts", post.Id)!;

        var content = post.Content;
        post.Content = null!;
        post.UpdatedAt = DateTime.UtcNow;

        File.WriteAllText(Path.Combine(postPath, "meta.json"), JsonSerializer.Serialize(post, _jsonSerializerOptions));
        File.WriteAllText(Path.Combine(postPath, "content.md"), content);

        _postResolver.UpdatePost(existingPost.Slug, post.Slug, post.Id, post.Status);

        UpdateCategoryFile(existingPost, post);
        UpdateTagFile(existingPost, post);
        UpdateEditors(existingPost, post);

        post.Content = content; // Restore content for return
        return post.Id;
    }

    public void DeletePost(Post post)
    {
        var existingPost = GetPostById(post.Id)!;
        var postPath = Path.Combine("Content", "posts", post.Id)!;

        UpdateCategoryFile(existingPost, new Post { Id = post.Id });
        UpdateTagFile(existingPost, new Post { Id = post.Id });
        RemovePostFromUser(existingPost.AuthorId ?? string.Empty, post.Id);
        UpdateEditors(existingPost, new Post { Id = post.Id });

        Directory.Delete(postPath, true);
        _postResolver.RemoveSlug(post.Slug);
    }
    #endregion

    #region Post existence checks
    public bool PostExists(string id)
    {
        var path = Path.Combine("Content", "posts", id);
        return Directory.Exists(path) && File.Exists(Path.Combine(path, "meta.json"));
    }

    public bool PostSlugExists(string slug, string postId)
    {
        var existingPost = GetPostBySlug(slug);
        return existingPost != null && existingPost.Id != postId;
    }
    #endregion

    #region Private methods
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

    private void UpdateEditors(Post post)
    {
        // Ensure editors are initialized
        if (post.Editors == null || post.Editors.Count == 0)
        {
            return;
        }

        // Add editors to the post
        foreach (var editorId in post.Editors)
        {
            var editorPath = Path.Combine("Content", "users", editorId, "profile.json");
            if (!File.Exists(editorPath))
            {
                continue;
            }

            string json = File.ReadAllText(editorPath);
            User editor = JsonSerializer.Deserialize<User>(json, _jsonSerializerOptions)!;

            if (!editor.Posts.Contains(post.Id))
            {
                editor.Posts = [.. editor.Posts, post.Id];
                File.WriteAllText(editorPath, JsonSerializer.Serialize(editor, _jsonSerializerOptions));
            }
        }
    }

    private void UpdateEditors(Post oldPost, Post newPost)
    {
        if (oldPost.Editors == null || oldPost.Editors.Count == 0)
        {
            oldPost.Editors = [];
        }

        // Remove editors from old post
        foreach (var editorId in oldPost.Editors)
        {
            var editorPath = Path.Combine("Content", "users", editorId, "profile.json");
            if (!File.Exists(editorPath))
            {
                continue;
            }

            string json = File.ReadAllText(editorPath);
            User editor = JsonSerializer.Deserialize<User>(json, _jsonSerializerOptions)!;

            if (editor.Posts.Contains(oldPost.Id))
            {
                editor.Posts = editor.Posts.Where(id => id != oldPost.Id).ToArray();
                File.WriteAllText(editorPath, JsonSerializer.Serialize(editor, _jsonSerializerOptions));
            }
        }

        // Add editors to new post
        foreach (var editorId in newPost.Editors)
        {
            var editorPath = Path.Combine("Content", "users", editorId, "profile.json");
            if (!File.Exists(editorPath))
            {
                continue;
            }

            string json = File.ReadAllText(editorPath);
            User editor = JsonSerializer.Deserialize<User>(json, _jsonSerializerOptions)!;

            if (!editor.Posts.Contains(newPost.Id))
            {
                editor.Posts = [.. editor.Posts, newPost.Id];
                File.WriteAllText(editorPath, JsonSerializer.Serialize(editor, _jsonSerializerOptions));
            }
        }
    }

    private void AddPostToUser(string userId, string postId)
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

    private void RemovePostFromUser(string userId, string postId)
    {
        var userPath = Path.Combine("Content", "users", userId, "profile.json");
        if (!File.Exists(userPath))
        {
            return;
        }

        string json = File.ReadAllText(userPath);
        User user = JsonSerializer.Deserialize<User>(json, _jsonSerializerOptions)!;

        if (user.Posts.Contains(postId))
        {
            user.Posts = user.Posts.Where(id => id != postId).ToArray();
            File.WriteAllText(userPath, JsonSerializer.Serialize(user, _jsonSerializerOptions));
        }
    }

    private void SaveCommentsForPost(string postId, List<Comment> comments)
    {
        var commentsPath = Path.Combine("Content", "posts", postId, "comments.json");
        var commentsData = new CommentsJson
        {
            Comments = comments.ToArray()
        };
        var json = JsonSerializer.Serialize(commentsData, _jsonSerializerOptions);
        File.WriteAllText(commentsPath, json);
    }
    #endregion
}

internal class CommentsJson
{
    public Comment[] Comments { get; set; } = [];
}
