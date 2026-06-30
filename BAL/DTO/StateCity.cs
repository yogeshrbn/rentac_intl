using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class StateDTO
    {
        public int StateId { get; set; }
        public string StateName { get; set; }
        public string GSTCode { get; set; }
        public virtual List<CityDTO> Cities { get; set; }
    }
    public class CityDTO
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
        public virtual StateDTO State { get; set; }
    }
}
