using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public int RbnClientId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string Company { get; set; }
        public Int16 RoleId { get; set; }
        public bool AllowSwitchCompany { get; set; }
        public int DefaultCompanyId { get; set; }
        public bool Active { get; set; }
        public int FinYearId { get; set; }
        public int DefaultWarehouseId { get; set; }

        public DateTime FinYearStart { get; set; }
        public DateTime FinYearEnd { get; set; }
        public string ProfilePic { get; set; }

        public int CompanyStateId { get; set; }
        //whatsapp enabled or not on the company
        public short wsApp { get; set; }
        public string RoleName { get; set; }
        public string Companies { get; set; }

    }

    public class ChangePasswordDTO
    {

        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }

    }

    public class ResetPasswordDTO
    {

        public string GuId { get; set; }
        public string Password { get; set; }

    }
}
