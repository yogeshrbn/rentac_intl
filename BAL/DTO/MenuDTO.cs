using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class MenuDTO
    {
        public string Title { get; set; }
        public int MenuId { get; set; }
        public string Url { get; set; }
        public int ParentMenuId { get; set; }
        public List<MenuDTO> ChildMenus { get; set; }
        public string IconClass { get; set; }
        public byte IsActive { get; set; }
        public short SortOrder { get; set; }
    }
}
