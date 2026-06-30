using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class ZonesDTO : BaseDTO
    {
        public string Name { get; set; }
        public int ZoneId { get; set; }

        public List<LocalityDTO> Localities { get; set; }

    }

     public class LocalityDTO  
    {
        public int LocalityId { get; set; }
        public string Name { get; set; }
        public byte Deleted { get; set; }
    }

}
