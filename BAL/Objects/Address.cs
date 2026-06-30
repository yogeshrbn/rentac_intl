using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.Enums;
using BAL.DAL;
namespace BAL.Objects
{
    public class Address
    {
        public List<AddressDTO> GetAddresses(AddressRoleType roleType, int hoderId)
        {
            AddressDAL dal = new AddressDAL();
            return dal.GetAddresses(roleType, hoderId);
        }
        public bool Save(AddressDTO dto)
        {
            AddressDAL dal = new AddressDAL();
            return dal.Save(null, dto);
        }
    }
}
