using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DAL;
namespace BAL.Objects
{
    public class Employee : EmployeeDTO
    {
        public bool Save()
        {
            EmployeeDAL dal = new EmployeeDAL();
            dal.Save(this);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<EmployeeDTO> GetAll(int companyId)
        {
            EmployeeDAL dal = new EmployeeDAL();
            return dal.GetAll(companyId);
        }

        public Employee()
        {

        }
        public Employee(int employeeId)
        {
            if (employeeId > 0)
            {
                EmployeeDAL dal = new EmployeeDAL();
                EmployeeDTO dto = dal.GetById(employeeId);
                this.Aadhar = dto.Aadhar;
                this.Address = dto.Address;
                this.Phone = dto.Phone;
                this.CompanyId = dto.CompanyId;
                this.EmployeeCode = dto.EmployeeCode;
                this.EmployeeId = dto.EmployeeId;
                this.Name = dto.Name;
                this.StatusId = dto.StatusId;
                this.RoleId = dto.RoleId;
                this.StatusName = dto.StatusName;
                this.RoleName = dto.RoleName;
            }
        }

        public async Task<int> SaveTeam(TeamDto dto)
        {
            EmployeeDAL dal = new EmployeeDAL();
            return await dal.SaveTeam(dto);
        }

        public async Task<IEnumerable<TeamDto>> TeamList(TeamDto dto)
        {
            EmployeeDAL dal = new EmployeeDAL();
            return await dal.TeamList(dto);
        }

        public async Task<TeamDto> TeamById(TeamDto dto)
        {
            EmployeeDAL dal = new EmployeeDAL();
            return await dal.TeamById(dto);
        }
        public async Task<int> DeleteTeam(TeamDto dto)
        {
            EmployeeDAL dal = new EmployeeDAL();
            return await dal.DeleteTeam(dto);
        }
    }
}
