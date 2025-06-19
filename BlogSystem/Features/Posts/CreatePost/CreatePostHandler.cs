using System.Security.Claims;
using BlogSystem.Domain.Entities;
using BlogSystem.Features.Categories.Data;
using BlogSystem.Features.Posts.CreatePost.DTOs;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Tags.Data;
using BlogSystem.Infrastructure.ImageService;
using BlogSystem.Shared.Exceptions;
using BlogSystem.Shared.Exceptions.Categories;
using BlogSystem.Shared.Exceptions.Posts;
using BlogSystem.Shared.Exceptions.Tags;
using BlogSystem.Shared.Helpers;

namespace BlogSystem.Features.Posts.CreatePost
{
    public class CreatePostHandler : ICreatePostHandler
    {
        private readonly IPostRepository _postRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;
        private readonly PostImageProvider _image;
        public CreatePostHandler(IPostRepository postRepository, ICategoryRepository categoryRepository, ITagRepository tagRepository, PostImageProvider image)
        {
            _postRepository = postRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _image = image;
        }

        public async Task<CreatePostResponseDTO> CreatePostAsync(CreatePostRequestDTO request, ClaimsPrincipal user)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ValidationErrorException("Title cannot be null or empty.");
            }

            var postId = SlugHelper.GenerateUniqueSlug(request.Title, _postRepository.PostExists);

            // Validate the request
            if (!string.IsNullOrEmpty(request.Slug) && _postRepository.PostExists(request.Slug))
            {
                throw new PostSlugAlreadyExistException(request.Slug);
            }

            if (!string.IsNullOrEmpty(request.Category) && _categoryRepository.CategoryExists(request.Category) == false)
            {
                throw new CategoryNotFoundException(request.Category);
            }

            if (request.Tags != null && request.Tags.Length != 0 && request.Tags.Length > 0)
            {
                foreach (var tag in request.Tags)
                {
                    if (!_tagRepository.TagExists(tag))
                    {
                        throw new TagNotFoundException(tag);
                    }
                }
            }

            if (request.IsPublished == true &&
                (string.IsNullOrWhiteSpace(request.Content) ||
                 string.IsNullOrWhiteSpace(request.Title) ||
                 string.IsNullOrWhiteSpace(request.Description) ||
                 string.IsNullOrWhiteSpace(request.Category) ||
                 string.IsNullOrWhiteSpace(request.Image?.FileName))
                )
            {
                throw new ValidationErrorException("Content, title, description, category, and image are required for published posts.");
            }

            var imagePath = string.Empty;
            if (request.Image != null && request.Image.Length > 0)
            {
                if (_image.IsValidImage(request.Image) == false)
                {
                    throw new ValidationErrorException("Invalid image format. Only JPEG, PNG, and GIF are allowed.");
                }

                var imageUrl = await _image.SaveImageAsync(request.Image, postId);
                imagePath = $"/images/posts/{postId}/{imageUrl}";
            }

            var post = new Post
            {
                Id = postId,
                Title = request.Title,
                Description = request.Description ?? string.Empty,
                AuthorId = user.FindFirstValue("Id")!,
                Category = request.Category ?? string.Empty,
                ImageUrl = imagePath,
                Slug = string.IsNullOrEmpty(request.Slug) ? postId : request.Slug,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Tags = (request.Tags ?? []).ToList(),
                IsPublished = request.IsPublished ?? false,
            };

            _postRepository.CreatePost(post, request.Content ?? string.Empty);
            return new CreatePostResponseDTO
            {
                Slug = post.Slug,
            };
        }
    }
}
