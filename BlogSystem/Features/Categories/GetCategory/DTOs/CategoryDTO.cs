namespace BlogSystem.Features.Categories.GetCategory.DTOs
{
    public class CategoryDTO
    {
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string[] Posts { get; set; } = [];
    }
}