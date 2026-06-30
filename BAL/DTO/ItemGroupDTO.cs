using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class ItemGroupDTO : BaseDTO
    {
        public int ItemGroupId { get; set; }

        public string GroupName { get; set; }
        public string GroupCode { get; set; }

    }
}
