using BlogSystem.Features.Users.UpdateUser.DTOs;
using FluentValidation;

namespace BlogSystem.Features.Users.UpdateUser.Validators;

public class UpdateProfileInfoRequestValidator : AbstractValidator<UpdateProfileInfoRequestDTO>
{
    public UpdateProfileInfoRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MinimumLength(5)
            .WithMessage("Username must be at least 5 characters long.")
            .MaximumLength(50)
            .WithMessage("Username must not exceed 50 characters.");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.")
            .Length(5, 100)
            .WithMessage("Full name must be between 5 and 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("A valid email is required.");

        RuleFor(x => x.Bio)
            .MaximumLength(500)
            .WithMessage("Bio cannot exceed 500 characters.");

        RuleFor(x => x.SocialLinks)
            .Must(links => links == null || links.All(kvp => Uri.IsWellFormedUriString(kvp.Value, UriKind.Absolute)))
            .WithMessage("All social links must be valid URLs.");
    }
}
