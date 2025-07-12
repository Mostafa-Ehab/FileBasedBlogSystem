using BlogSystem.Shared.Exceptions;
using FluentValidation;
using System.Text.RegularExpressions;

namespace BlogSystem.Shared.Helpers;

public static class ValidationHelper
{
    public static readonly Regex SlugRegex = new(
        @"^[a-z0-9]+(?:-[a-z0-9]+)*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    );

    public static void Validate<T>(T request, IValidator<T> validator)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationErrorException(
                string.Join(", ", validationResult.Errors.Select(vr => vr.ErrorMessage).ToList())
            );
        }
    }
}