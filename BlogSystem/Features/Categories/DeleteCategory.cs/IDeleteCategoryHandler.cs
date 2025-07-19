namespace BlogSystem.Features.Categories.DeleteCategory;

public interface IDeleteCategoryHandler
{
    Task DeleteCategoryAsync(string slug);
}
