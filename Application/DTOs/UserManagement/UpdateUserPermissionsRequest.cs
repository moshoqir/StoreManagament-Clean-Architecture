namespace Application.DTOs.UserManagement
{
    public sealed class UpdateUserPermissionsRequest
    {
        public List<UserPermissionItemRequest> Permissions { get; set; } = [];
    }
}
