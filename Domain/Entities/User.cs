using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public bool IsFirstLogin { get; set; }
        public DateTime? LastLogin { get; set; }
       
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }

        public List<Role> Roles { get; set; } = [];
    }
}
