namespace BlogSystem.Features.Users.Login.DTOs
{
    public record LoginResponseDTO
    {
        public string AccessToken { get; set; } = string.Empty;
    }
}
