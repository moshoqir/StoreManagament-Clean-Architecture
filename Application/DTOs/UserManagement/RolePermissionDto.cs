namespace Application.DTOs.UserManagement
{
     
    public sealed class RolePermissionDto
    {
        public int MenuActionId { get; init; }
        public string MenuCode { get; init; } = string.Empty;
        public string MenuNameAr { get; init; } = string.Empty;
        public string MenuNameEn { get; init; } = string.Empty;
        public string ActionNameAr { get; init; } = string.Empty;
        public string ActionNameEn { get; init; } = string.Empty;
        public string PermissionCode { get; init; } = string.Empty;
        public bool IsAllowed { get; init; }
    }
}
