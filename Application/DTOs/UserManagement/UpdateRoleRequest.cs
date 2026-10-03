using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.UserManagement
{
    public sealed class UpdateRoleRequest
    {
        [Required(ErrorMessage = "Arabic role name is required.")]
        [StringLength(150)]
        public string RoleNameAr { get; set; } = string.Empty;

        [Required(ErrorMessage = "English role name is required.")]
        [StringLength(150)]
        public string RoleNameEn { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
