using BlogSystem.Features.Tags.CreateTag.DTOs;
using FluentValidation;

namespace BlogSystem.Features.Tags.CreateTag.Validators;

public class CreateTagRequestValidator : AbstractValidator<CreateTagRequestDTO>
{
    public CreateTagRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");
    }
}
