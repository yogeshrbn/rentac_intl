using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class AccountGroupDTO
    {
        public int AccountGroupId { get; set; }
        public string Name { get; set; }
        public bool PrimaryGroup { get; set; }
        public bool SubGroup { get; set; }
        public int ParentGroup { get; set; }
        public bool Editable { get; set; }
        public int StoreId { get; set; }
        public string GroupCode { get; set; }
        public bool IsActive { get; set; }
    }

}
