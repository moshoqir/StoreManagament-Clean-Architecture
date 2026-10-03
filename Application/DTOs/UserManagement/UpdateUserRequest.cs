using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.UserManagement
{
    public sealed class UpdateUserRequest
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 200 characters.")]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email format is invalid.")]
        [StringLength(200)]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? PhoneNumber { get; set; }
 
        [StringLength(100, MinimumLength = 6, ErrorMessage = "New password must be at least 6 characters.")]
        public string? NewPassword { get; set; }

        public bool IsActive { get; set; } = true;
        public List<int> RoleIds { get; set; } = [];
    }
}
