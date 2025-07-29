using BlogSystem.Shared.Exceptions;
using BlogSystem.Shared.Helpers;
using Microsoft.Extensions.FileProviders;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Resolvers;

namespace BlogSystem.Infrastructure.ImageService;

public class PostContentImageProvider : IImageProvider
{
    private readonly IFileProvider _fileProvider;
    private const string PostImagesPrefix = "/images/posts/";
    private const int ExpectedSegmentCount = 3;

    public PostContentImageProvider()
    {
        _fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
    }

    #region IImageProvider Implementation
    public ProcessingBehavior ProcessingBehavior => ProcessingBehavior.All;

    public Func<HttpContext, bool> Match { get; set; } = context =>
    {
        var path = context.Request.Path.Value;
        return !string.IsNullOrEmpty(path) &&
               path.StartsWith(PostImagesPrefix, StringComparison.OrdinalIgnoreCase) &&
               path.Count(c => c == '/') == ExpectedSegmentCount;
    };

    public Task<IImageResolver?> GetAsync(HttpContext context)
    {
        if (!TryExtractImageInfo(context.Request.Path.Value, out var imageName))
        {
            return Task.FromResult<IImageResolver?>(null);
        }

        var imagePath = Path.Combine("Content", "images", imageName);
        var fileInfo = _fileProvider.GetFileInfo(imagePath);

        return Task.FromResult<IImageResolver?>(
            fileInfo.Exists ? new FileProviderImageResolver(fileInfo) : null
        );
    }

    public bool IsValidRequest(HttpContext context)
    {
        return TryExtractImageInfo(context.Request.Path.Value, out var imageName);
    }
    #endregion

    #region Image Saving
    public async Task<string> SaveImageAsync(IFormFile imageFile)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
        {
            throw new ValidationErrorException("Invalid image file type. Allowed types are: " + string.Join(", ", allowedExtensions));
        }

        var randomName = ImageHelper.GetRandomFileName(fileExtension);
        var postDirectory = Path.Combine("Content", "images");
        var fullPath = Path.Combine(postDirectory, randomName);

        if (!Directory.Exists(postDirectory))
        {
            Directory.CreateDirectory(postDirectory);
        }

        while (File.Exists(fullPath))
        {
            randomName = ImageHelper.GetRandomFileName(fileExtension);
            fullPath = Path.Combine(postDirectory, randomName);
        }

        using var stream = new FileStream(fullPath, FileMode.Create);
        await imageFile.CopyToAsync(stream);

        return $"/images/posts/{randomName}";
    }
    #endregion

    #region Helper Methods
    private static bool TryExtractImageInfo(string? path, out string imageName)
    {
        imageName = string.Empty;

        if (string.IsNullOrEmpty(path) ||
            !path.StartsWith(PostImagesPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length != ExpectedSegmentCount)
        {
            return false;
        }

        imageName = segments[2];
        return true;
    }
    #endregion
}