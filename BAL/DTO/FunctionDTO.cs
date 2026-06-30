using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace BAL.DTO
{
    public class FunctionDTO
    {
        public int FunctionId { get; set; }
        public string Name { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool View { get; set; }
        public bool Delete { get; set; }
        public string Route { get; set; }

        public int MenuId { get; set; }
        public int ParentId { get; set; }
        public int RoleFunctionId { get; set; }
        public List<FunctionDTO> Children { get; set; }
    }
}
