using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Menu
    {
        public int MenuId { get; set; }
        public string MenuNameEn { get; set; } = string.Empty;
        public string MenuNameAr { get; set; } = string.Empty;
        public string MenuCode { get; set; } = string.Empty;
        public string? MenuUrl { get; set; }
        public int? MenuParentId { get; set; }
        public string? MenuIcon { get; set; }
        public bool MenuNavHeader { get; set; }
        public int MenuParentOrder { get; set; }
        public int MenuChildOrder { get; set; }

        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
