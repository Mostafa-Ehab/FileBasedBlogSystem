using BlogSystem.Features.Posts.Data;

namespace BlogSystemTest.Posts.Data
{
    public class GetPostBySlugTest
    {
        [Fact]
        public void GetPost_ShouldReturnPost_WhenPostExists()
        {
            // Arrange
            var postSlug = "first-post";

            var expectedTitle = "First Post";
            var expectedDescription = "This is the first post in our new blog system.";
            var expectedTags = new List<string> { "introduction", "welcome" };
            var expectedAuthor = "Jane Doe";
            var expectedSlug = "first-post";

            // Act
            var postRepository = new PostRepository();
            var post = postRepository.GetPostBySlug(postSlug);

            // Assert
            Assert.NotNull(post);
            Assert.Equal(expectedTitle, post.Title);
            Assert.Equal(expectedDescription, post.Description);
            Assert.Equivalent(expectedTags, post.Tags);
            Assert.Equal(expectedAuthor, post.Author);
            Assert.Equal(expectedSlug, post.Slug);
        }

        [Fact]
        public void GetPost_ShouldReturnNull_WhenPostDoesNotExist()
        {
            // Arrange
            var postSlug = "non-existent-post";

            // Act
            var postRepository = new PostRepository();
            var post = postRepository.GetPostBySlug(postSlug);

            // Assert
            Assert.Null(post);
        }

        [Fact]
        public void GetPost_ShouldReturnPostContent_WhenPostExists()
        {
            // Arrange
            var postSlug = "first-post";
            var postId = "2025-06-09-first-post";
            var path = Path.Combine(AppContext.BaseDirectory, "Content", "posts", postId);
            var expectedContent = File.ReadAllText(Path.Combine(path, "content.md"));

            // Act
            var postRepository = new PostRepository();
            var post = postRepository.GetPostBySlug(postSlug);

            // Assert
            Assert.NotNull(post);
            Assert.Equal(expectedContent, post.Content);
        }
    }
}
