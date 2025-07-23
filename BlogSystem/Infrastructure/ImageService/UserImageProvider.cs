using BlogSystem.Shared.Exceptions;
using BlogSystem.Shared.Helpers;
using Microsoft.Extensions.FileProviders;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Resolvers;

namespace BlogSystem.Infrastructure.ImageService;

public class UserImageProvider : IImageProvider
{
    private readonly IFileProvider _fileProvider;
    private const string UserImagesPrefix = "/images/users/";
    private const int ExpectedSegmentCount = 4;

    public UserImageProvider()
    {
        _fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
    }

    #region IImageProvider Implementation
    public ProcessingBehavior ProcessingBehavior => ProcessingBehavior.All;

    public Func<HttpContext, bool> Match { get; set; } = context =>
    {
        var path = context.Request.Path.Value;
        return !string.IsNullOrEmpty(path) &&
               path.StartsWith(UserImagesPrefix, StringComparison.OrdinalIgnoreCase) &&
               path.Count(c => c == '/') == ExpectedSegmentCount;
    };

    public Task<IImageResolver?> GetAsync(HttpContext context)
    {
        if (!TryExtractImageInfo(context.Request.Path.Value, out var userId, out var imageName))
        {
            return Task.FromResult<IImageResolver?>(null);
        }

        var imagePath = Path.Combine("Content", "users", userId, "assets", imageName);
        var fileInfo = _fileProvider.GetFileInfo(imagePath);

        return Task.FromResult<IImageResolver?>(
            fileInfo.Exists ? new FileProviderImageResolver(fileInfo) : null
        );
    }

    public bool IsValidRequest(HttpContext context)
    {
        return TryExtractImageInfo(context.Request.Path.Value, out var userId, out _) &&
               SlugHelper.ValidateSlug(userId);
    }
    #endregion

    #region Image Saving
    public async Task<string> SaveImageAsync(IFormFile imageFile, string userId)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
        {
            throw new ValidationErrorException("Invalid image file type. Allowed types are: " + string.Join(", ", allowedExtensions));
        }

        var randomName = ImageHelper.GetRandomFileName(fileExtension);
        var userDirectory = Path.Combine("Content", "users", userId, "assets");
        var fullPath = Path.Combine(userDirectory, randomName);

        if (!Directory.Exists(userDirectory))
        {
            Directory.CreateDirectory(userDirectory);
        }

        while (File.Exists(fullPath))
        {
            fullPath = Path.Combine(userDirectory, ImageHelper.GetRandomFileName(fileExtension));
        }

        using var stream = new FileStream(fullPath, FileMode.Create);
        await imageFile.CopyToAsync(stream);

        return $"/images/users/{userId}/{randomName}";
    }
    #endregion

    #region Helper Methods
    private static bool TryExtractImageInfo(string? path, out string userId, out string imageName)
    {
        userId = string.Empty;
        imageName = string.Empty;

        if (string.IsNullOrEmpty(path) ||
            !path.StartsWith(UserImagesPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length != ExpectedSegmentCount)
        {
            return false;
        }

        userId = segments[2];
        imageName = segments[3];
        return true;
    }
    #endregion
}
