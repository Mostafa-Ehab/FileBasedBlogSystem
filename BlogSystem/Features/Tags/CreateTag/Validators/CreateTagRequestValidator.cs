using FluentValidation;
using BlogSystem.Features.Tags.CreateTag.DTOs;
using BlogSystem.Features.Tags.Data;

namespace BlogSystem.Features.Tags.CreateTag.Validators
{
    public class CreateTagRequestValidator : AbstractValidator<CreateTagRequestDTO>
    {
        public CreateTagRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.");
        }
    }
}
