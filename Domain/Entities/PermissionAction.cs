using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PermissionAction
    {
        public int ActionId { get; set; }
        public string ActionNameEn { get; set; } = string.Empty;

        public string ActionNameAr { get; set; } = string.Empty;
    }
}
