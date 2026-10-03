namespace Application.DTOs.UserManagement
{
    public sealed class RoleDto
    {
        public int RoleId { get; init; }
        public string RoleNameAr { get; init; } = string.Empty;
        public string RoleNameEn { get; init; } = string.Empty;
        public bool IsActive { get; init; }
    }
}
