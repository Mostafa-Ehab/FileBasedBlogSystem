namespace BlogSystem.Shared.Helpers;

public static class ImageHelper
{
    public static string GetRandomProfilePictureUrl()
    {
        return $"https://picsum.photos/200/200?random={Guid.NewGuid()}";
    }
}
