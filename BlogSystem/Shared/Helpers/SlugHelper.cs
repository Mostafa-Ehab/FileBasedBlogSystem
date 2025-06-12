using System.Text.RegularExpressions;

namespace BlogSystem.Shared.Helpers
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string input, int maxLength = 20)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input cannot be empty.", nameof(input));

            // Remove special characters and convert to lowercase
            var baseSlug = Regex.Replace(input.ToLowerInvariant(), "[^a-z0-9-]", "-")
                .Trim('-');

            // Truncate to maxLength
            return baseSlug.Length > maxLength ? baseSlug[..maxLength] : baseSlug;
        }

        public static string GenerateUniqueSlug(string input, Func<string, bool> isTaken, int maxLength = 20)
        {
            var baseSlug = GenerateSlug(input, maxLength);
            var uniqueSlug = baseSlug;
            var counter = 1;
            while (isTaken(uniqueSlug))
            {
                uniqueSlug = $"{baseSlug}{counter++}";
                if (uniqueSlug.Length > maxLength) uniqueSlug = uniqueSlug[..maxLength];
            }
            return uniqueSlug;
        }
    }
}