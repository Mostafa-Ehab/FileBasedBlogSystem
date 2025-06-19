using FluentValidation;
using BlogSystem.Features.Posts.UpdatePost.DTOs;
using BlogSystem.Shared.Helpers;

namespace BlogSystem.Features.Posts.UpdatePost.Validators
{
    public class UpdatePostRequestValidator : AbstractValidator<UpdatePostRequestDTO>
    {
        public UpdatePostRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.");

            RuleFor(x => x.Slug)
                .NotEmpty()
                .WithMessage("Slug is required.")
                .Matches(ValidationHelper.SlugRegex)
                .WithMessage("Slug must be a valid slug format.");
        }
    }
}