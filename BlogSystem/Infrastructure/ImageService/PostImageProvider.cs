using BlogSystem.Shared.Helpers;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Resolvers;
using Microsoft.Extensions.FileProviders;
using BlogSystem.Shared.Exceptions;
using SixLabors.ImageSharp;

namespace BlogSystem.Infrastructure.ImageService;

public class PostImageProvider : IImageProvider
{
    private readonly IFileProvider _fileProvider;
    private const string PostImagesPrefix = "/images/posts/";
    private const int ExpectedSegmentCount = 4;

    public PostImageProvider()
    {
        _fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
    }

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

    public async Task<string> SaveImageAsync(IFormFile imageFile, string postSlug)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
        {
            throw new ValidationErrorException("Invalid image file type. Allowed types are: " + string.Join(", ", allowedExtensions));
        }

        var randomName = $"{Path.GetRandomFileName()}{fileExtension}";
        var postDirectory = Path.Combine("Content", "posts", postSlug, "assets");
        var fullPath = Path.Combine(postDirectory, randomName);

        if (!Directory.Exists(postDirectory))
        {
            Directory.CreateDirectory(postDirectory);
        }

        while (File.Exists(fullPath))
        {
            fullPath = Path.Combine(postDirectory, $"{Path.GetRandomFileName()}{fileExtension}");
        }

        using var stream = new FileStream(fullPath, FileMode.Create);
        await imageFile.CopyToAsync(stream);

        return randomName;
    }

    public bool IsValidImage(IFormFile file)
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
}