using System.Security.Claims;
using BlogSystem.Domain.Entities;
using BlogSystem.Features.Categories.Data;
using BlogSystem.Features.Posts.UpdatePost.DTOs;
using BlogSystem.Features.Posts.Data;
using BlogSystem.Features.Tags.Data;
using BlogSystem.Infrastructure.ImageService;
using BlogSystem.Shared.Exceptions;
using BlogSystem.Shared.Exceptions.Categories;
using BlogSystem.Shared.Exceptions.Posts;
using BlogSystem.Shared.Exceptions.Tags;
using FluentValidation;

namespace BlogSystem.Features.Posts.UpdatePost
{
    public class UpdatePostHandler : IUpdatePostHandler
    {
        private readonly IPostRepository _postRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;
        private readonly PostImageProvider _image;
        private readonly IValidator<UpdatePostRequestDTO> _validator;
        public UpdatePostHandler(IPostRepository postRepository, ICategoryRepository categoryRepository, ITagRepository tagRepository, PostImageProvider image, IValidator<UpdatePostRequestDTO> validator)
        {
            _postRepository = postRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _image = image;
            _validator = validator;
        }

        public async Task<UpdatePostResponseDTO> HandleUpdatePostAsync(UpdatePostRequestDTO request, string postId, ClaimsPrincipal user)
        {
            var currentPost = _postRepository.GetPostById(postId) ?? throw new PostNotFoundException(postId);

            if (currentPost.AuthorId != user.FindFirstValue("Id") && !user.IsInRole("Admin") && !user.IsInRole("Editor"))
            {
                throw new UnauthorizedAccessException("You are not authorized to update this post.");
            }

            if (currentPost.Slug != request.Slug && _postRepository.PostExists(request.Slug))
            {
                throw new PostSlugAlreadyExistException(request.Slug);
            }

            Post postToUpdate = new Post
            {
                Id = postId,
                Title = request.Title,
                Description = request.Description,
                AuthorId = currentPost.AuthorId,
                Category = request.Category,
                ImageUrl = currentPost.ImageUrl,
                Slug = request.Slug,
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = currentPost.CreatedAt,
                IsPublished = request.IsPublished,
                Tags = request.Tags.ToList(),
            };

            if (postToUpdate.IsPublished &&
                (string.IsNullOrWhiteSpace(request.Content) ||
                 string.IsNullOrWhiteSpace(postToUpdate.Title) ||
                 string.IsNullOrWhiteSpace(postToUpdate.Description) ||
                 string.IsNullOrWhiteSpace(postToUpdate.Category) ||
                 (string.IsNullOrWhiteSpace(request.Image?.FileName) && string.IsNullOrWhiteSpace(postToUpdate.ImageUrl))
                ))
            {
                throw new ValidationErrorException("Content, title, description, category, and image are required for published posts.");
            }

            if (request.Image != null && request.Image.Length > 0)
            {
                if (_image.IsValidImage(request.Image) == false)
                {
                    throw new ValidationErrorException("Invalid image format. Only JPEG, PNG, and GIF are allowed.");
                }

                var imageUrl = await _image.SaveImageAsync(request.Image, postId);
                postToUpdate.ImageUrl = $"/images/posts/{postId}/{imageUrl}";
            }

            if (postToUpdate.Category != currentPost.Category &&
                !_categoryRepository.CategoryExists(postToUpdate.Category))
            {
                throw new CategoryNotFoundException(postToUpdate.Category);
            }

            if (postToUpdate.Tags.Count > 0)
            {
                foreach (var tag in postToUpdate.Tags)
                {
                    if (!_tagRepository.TagExists(tag))
                    {
                        throw new TagNotFoundException(tag);
                    }
                }
            }

            _postRepository.UpdatePost(postToUpdate, request.Content);
            return new UpdatePostResponseDTO
            {
                Id = postId,
                Slug = postToUpdate.Slug,
            };
        }
    }
}