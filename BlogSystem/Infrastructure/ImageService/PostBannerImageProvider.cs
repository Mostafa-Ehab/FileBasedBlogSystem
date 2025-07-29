using BlogSystem.Shared.Exceptions;
using BlogSystem.Shared.Helpers;
using Microsoft.Extensions.FileProviders;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Resolvers;

namespace BlogSystem.Infrastructure.ImageService;

public class PostBannerImageProvider : IImageProvider
{
    private readonly IFileProvider _fileProvider;
    private const string PostImagesPrefix = "/images/posts/";
    private const int ExpectedSegmentCount = 4;

    public PostBannerImageProvider()
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
        if (!TryExtractImageInfo(context.Request.Path.Value, out var postSlug, out var imageName))
        {
            return Task.FromResult<IImageResolver?>(null);
        }

        var imagePath = Path.Combine("Content", "posts", postSlug, "assets", imageName);
        var fileInfo = _fileProvider.GetFileInfo(imagePath);

        return Task.FromResult<IImageResolver?>(
            fileInfo.Exists ? new FileProviderImageResolver(fileInfo) : null
        );
    }

    public bool IsValidRequest(HttpContext context)
    {
        return TryExtractImageInfo(context.Request.Path.Value, out var postSlug, out _) &&
               SlugHelper.ValidateSlug(postSlug);
    }
    #endregion

    #region Image Saving
    public async Task<string> SaveImageAsync(IFormFile imageFile, string postSlug)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
        {
            throw new ValidationErrorException("Invalid image file type. Allowed types are: " + string.Join(", ", allowedExtensions));
        }

        var randomName = ImageHelper.GetRandomFileName(fileExtension);
        var postDirectory = Path.Combine("Content", "posts", postSlug, "assets");
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

        return $"/images/posts/{postSlug}/{randomName}";
    }
    #endregion

    #region Helper Methods
    private static bool TryExtractImageInfo(string? path, out string postSlug, out string imageName)
    {
        postSlug = string.Empty;
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

        postSlug = segments[2];
        imageName = segments[3];
        return true;
    }
    #endregion
}