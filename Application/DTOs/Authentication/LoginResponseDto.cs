namespace Application.DTOs.Authentication
{
 
    public sealed class LoginResponseDto
    {
        public int UserId { get; init; }
        public string UserName { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public string AccessToken { get; init; } = string.Empty;
        public IReadOnlyCollection<string> Permissions { get; init; } = [];
    }
}
