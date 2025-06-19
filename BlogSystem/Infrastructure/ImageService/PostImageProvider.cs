using BlogSystem.Shared.Helpers;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Resolvers;
using Microsoft.Extensions.FileProviders;

namespace BlogSystem.Infrastructure.ImageService
{
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
}