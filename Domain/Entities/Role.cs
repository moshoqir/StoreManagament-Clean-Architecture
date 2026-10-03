using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleNameEn { get; set; } = string.Empty;
        public string RoleNameAr { get; set; } = string.Empty;
        
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
