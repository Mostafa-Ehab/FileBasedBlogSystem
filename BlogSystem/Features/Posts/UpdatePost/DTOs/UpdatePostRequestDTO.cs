namespace BlogSystem.Features.Posts.UpdatePost.DTOs
{
    public class UpdatePostRequestDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
        public string Slug { get; set; } = string.Empty;
        public bool IsPublished { get; set; } = false;
        public string[] Tags { get; set; } = [];
    }
}