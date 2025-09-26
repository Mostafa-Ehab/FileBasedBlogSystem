using BlogSystem.Features.Users.Authentication.DTOs;
using FluentValidation;

namespace BlogSystem.Features.Users.Authentication.Validators;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequestDTO>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");

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
    }
}