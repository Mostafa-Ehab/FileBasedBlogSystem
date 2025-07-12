using BlogSystem.Features.Users.CreateUser.DTOs;
using BlogSystem.Shared.Helpers;
using FluentValidation;

namespace BlogSystem.Features.Users.CreateUser.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequestDTO>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required")
            .MinimumLength(5)
            .WithMessage("Username must be at least 5 characters long.")
            .MaximumLength(50)
            .WithMessage("Username must not exceed 50 characters.")
            .Matches(ValidationHelper.SlugRegex)
            .WithMessage("Username must contain only lowercase letters, numbers, and single hyphens (no leading/trailing or repeated hyphens).")
            .When(x => !string.IsNullOrWhiteSpace(x.Username));

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
        // .MinimumLength(8)
        // .WithMessage("Password must be at least 8 characters long.")
        // .Matches("[A-Z]")
        // .WithMessage("Password must contain at least one uppercase letter.")
        // .Matches("[a-z]")
        // .WithMessage("Password must contain at least one lowercase letter.")
        // .Matches("[0-9]")
        // .WithMessage("Password must contain at least one number.")
        // .Matches("[^a-zA-Z0-9]")
        // .WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required")
            .MaximumLength(100)
            .WithMessage("Full name must not exceed 100 characters.");

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Role must be one of the predefined user roles (Admin, Editor, or Author).")
            .When(x => x.Role.HasValue, ApplyConditionTo.CurrentValidator);
    }
}