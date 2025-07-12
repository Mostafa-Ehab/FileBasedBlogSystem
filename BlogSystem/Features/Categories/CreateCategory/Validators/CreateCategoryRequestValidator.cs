using BlogSystem.Features.Categories.CreateCategory.DTOs;
using FluentValidation;

namespace BlogSystem.Features.Categories.CreateCategory.Validators;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequestDTO>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.");
    }
}
