namespace BlogSystemTest.Posts.Data;

public class GetPostsByCategoryTest : UnitTestBase
{
    public GetPostsByCategoryTest()
    {
        SeedContent();
    }

    [Fact]
    public void GetPostsByCategory_ShouldReturnPosts_WhenCategoryExists()
    {
        // Arrange
        var postRepository = CreatePostRepository();
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
        var postRepository = CreatePostRepository();
        var slug = "non-existent-category";

        // Act
        var posts = postRepository.GetPostsByCategory(slug);

        // Assert
        Assert.NotNull(posts);
        Assert.Empty(posts);
    }
}
