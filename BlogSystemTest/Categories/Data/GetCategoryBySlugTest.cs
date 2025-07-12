namespace BlogSystemTest.Categories.Data;

public class GetCategoryBySlugTest : UnitTestBase
{
    public GetCategoryBySlugTest()
    {
        SeedContent();
    }

    [Fact]
    public void GetCategoryBySlug_ShouldReturnCategory_WhenCategoryExists()
    {
        // Arrange
        var categoryRepository = CreateCategoryRepository();
        var slug = "updates";
        var expectedName = "Updates";
        var expectedDescription = "Latest updates and news from our team.";
        var expectedPosts = new List<string> { "2025-06-09-second-post" };

        // Act
        var category = categoryRepository.GetCategoryBySlug(slug);

        // Assert
        Assert.NotNull(category);
        Assert.Equal(expectedName, category.Name);
        Assert.Equal(expectedDescription, category.Description);
        Assert.Equivalent(expectedPosts, category.Posts);
    }

    [Fact]
    public void GetCategoryBySlug_ShouldReturnNull_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryRepository = CreateCategoryRepository();
        var slug = "non-existent-category";

        // Act
        var category = categoryRepository.GetCategoryBySlug(slug);

        // Assert
        Assert.Null(category);
    }
}
