using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public sealed class PermissionItem
    {
        public int MenuActionId { get; set; }
        public string MenuCode { get; set; } = string.Empty;
        public string MenuNameAr { get; set; } = string.Empty;
        public string MenuNameEn { get; set; } = string.Empty;
        public string ActionNameAr { get; set; } = string.Empty;
        public string ActionNameEn { get; set; } = string.Empty;
        public string PermissionCode { get; set; } = string.Empty;
        public bool IsAllowed { get; set; }
        public bool RoleAllowed { get; set; }
        public bool? UserOverride { get; set; }
        public bool EffectiveAllowed { get; set; }

    }
}
