using BlogSystem.Features.Users.UpdateUser.DTOs;
using BlogSystem.Shared.Helpers;
using FluentValidation;

namespace BlogSystem.Features.Users.UpdateUser.Validators;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequestDTO>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required")
            .MinimumLength(5)
            .WithMessage("Username must be at least 5 characters long.")
            .MaximumLength(50)
            .WithMessage("Username must not exceed 50 characters.")
            .Matches(ValidationHelper.SlugRegex)
            .WithMessage("Username must contain only lowercase letters, numbers, and single hyphens (no leading/trailing or repeated hyphens).");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.")
            .Length(5, 100)
            .WithMessage("Full name must be between 5 and 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.Bio)
            .MaximumLength(500)
            .WithMessage("Bio cannot exceed 500 characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.")
            .When(x => !string.IsNullOrEmpty(x.Password));

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Role must be one of the predefined user roles (Admin, Editor, or Author).");
    }
}
