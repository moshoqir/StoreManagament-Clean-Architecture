namespace Application.DTOs.Authentication
{
 
    public sealed class AuthenticatedUserDto
    {
        public int UserId { get; init; }
        public string UserName { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public IReadOnlyCollection<string> Permissions { get; init; } = [];
    }
}
