namespace Application.DTOs.UserManagement
{
    public sealed class UserPermissionItemRequest
    {
        public int MenuActionId { get; set; }

        // null = Use Role, true = Allow, false = Deny.
        public bool? IsAllowed { get; set; }
    }
}
