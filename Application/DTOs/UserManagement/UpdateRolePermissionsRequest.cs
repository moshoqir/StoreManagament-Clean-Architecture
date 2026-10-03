namespace Application.DTOs.UserManagement
{
 
    public sealed class UpdateRolePermissionsRequest
    {
        public List<int> MenuActionIds { get; set; } = [];
    }
}
