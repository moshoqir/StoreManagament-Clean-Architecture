namespace Application.DTOs.UserManagement
{
 
    public sealed class UserDto
    {
        public int UserId { get; init; }
        public string UserName { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
        public bool IsActive { get; init; }
        public IReadOnlyCollection<int> RoleIds { get; init; } = [];
        public IReadOnlyCollection<string> RoleNames { get; init; } = [];
    }
}
