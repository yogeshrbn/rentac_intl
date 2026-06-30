using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class EmployeeDTO
    {
        public int EmployeeId { get; set; }
        public int StatusId { get; set; }
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Aadhar { get; set; }
        public string Phone { get; set; }
        public string EmployeeCode { get; set; }
        public string RoleName { get; set; }
        public string StatusName { get; set; }
        public int CompanyId { get; set; }
    }


    public class TeamDto : BaseDTO
    {
        public int TeamId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public List<EmployeeDTO> Employees { get; set; }
    }
}
