namespace BlogSystem.Features.Users.Login.DTOs
{
    public record LoginRequestDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}