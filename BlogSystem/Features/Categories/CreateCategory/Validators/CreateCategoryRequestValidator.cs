using FluentValidation;
using BlogSystem.Features.Categories.CreateCategory.DTOs;
using BlogSystem.Features.Categories.Data;

namespace BlogSystem.Features.Categories.CreateCategory.Validators
{
    public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequestDTO>
    {
        public CreateCategoryRequestValidator(ICategoryRepository categoryRepository)
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
