namespace BlogSystem.Features.Tags.DeleteTag;

public interface IDeleteTagHandler
{
    Task DeleteTagAsync(string slug);
}
