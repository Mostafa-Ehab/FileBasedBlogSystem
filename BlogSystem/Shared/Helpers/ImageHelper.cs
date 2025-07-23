using SixLabors.ImageSharp.Web.Resolvers;

namespace BlogSystem.Shared.Helpers;

public static class ImageHelper
{
    public static string GetRandomProfilePictureUrl()
    {
        return $"https://picsum.photos/200/200?random={Guid.NewGuid()}";
    }

    public static string GetRandomFileName(string extension)
    {
        return $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}{extension}";
    }

    public static bool IsValidImage(IFormFile file)
    {
        try
        {
            using (Image newImage = Image.Load(file.OpenReadStream()))
            {
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }
}
