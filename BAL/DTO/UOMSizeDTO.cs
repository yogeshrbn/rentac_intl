using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class UOMSizeDTO : UOMDTO
    {
        public int UOMSizeId { get; set; }
        public String Size { get; set; }
        public bool Active { get; set; }
    }
}
