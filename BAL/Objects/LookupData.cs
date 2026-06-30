using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DAL;
using BAL.Enums;
namespace BAL.Objects
{
    public class LookupData
    {
        public List<RoleDTO> GetAllRoles(RoleType type)
        {
            LookupDataDAL dal = new LookupDataDAL();
            return dal.GetRole(type);
        }
        public List<OtherChargeDTO> GetOtherCharges()
        {
            LookupDataDAL dal = new LookupDataDAL();
            return dal.GetOtherCharges();
        }
    }
}
