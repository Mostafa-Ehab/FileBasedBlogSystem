namespace BlogSystem.Shared.Exceptions.Categories;

public class CategoryNotFoundException : ApplicationCustomException
{
    public CategoryNotFoundException(string id) : base($"Category '{id}' not found.", 404, 404)
    {
    }
    public CategoryNotFoundException(string id, int errorCode) : base($"Category '{id}' not found.", 404, errorCode)
    {
    }
}