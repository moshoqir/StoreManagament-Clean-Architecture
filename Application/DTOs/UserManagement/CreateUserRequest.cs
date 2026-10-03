using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.UserManagement
{
    public sealed class CreateUserRequest
    {
        [Required(ErrorMessage = "User name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "User name must be between 3 and 100 characters.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 200 characters.")]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email format is invalid.")]
        [StringLength(200)]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

         public List<int> RoleIds { get; set; } = [];
    }
}
