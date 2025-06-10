using System.Text.Json;
using BlogSystem.Domain.Entities;
using BlogSystem.Features.Posts.Data;

namespace BlogSystemTest.Posts.Data
{
    public class GetPostsByCategoryTest
    {
        private readonly PostRepository postRepository;

        public GetPostsByCategoryTest()
        {
            postRepository = new PostRepository(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        [Fact]
        public void GetPostsByCategory_ShouldReturnPosts_WhenCategoryExists()
        {
            // Arrange
            var slug = "updates";
            var expectedPost = postRepository.GetPostById("2025-06-09-second-post");

            // Act
            var posts = postRepository.GetPostsByCategory(slug);

            // Assert
            Assert.NotNull(posts);
            Assert.Single(posts);
            Assert.Equivalent(expectedPost, posts[0]);
        }

        [Fact]
        public void GetPostsByCategory_ShouldReturnEmptyArray_WhenCategoryDoesNotExist()
        {
            // Arrange
            var slug = "non-existent-category";

            // Act
            var posts = postRepository.GetPostsByCategory(slug);

            // Assert
            Assert.NotNull(posts);
            Assert.Empty(posts);
        }
    }
}
