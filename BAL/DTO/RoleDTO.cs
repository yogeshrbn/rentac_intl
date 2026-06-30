using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class RoleDTO
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public int RoleTypeId { get; set; }
        List<FunctionDTO> _functions = new List<FunctionDTO>();
        public List<FunctionDTO> Functions
        {
            get { return _functions; }
            set { _functions = value; }
        }
    }
}
