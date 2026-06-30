using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class RBNClientDTO
    {
        public int RbnClientId { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string PinCode { get; set; }
        public string PAN { get; set; }
        public string GST { get; set; }
        public int? Stateid { get; set; }
        public int? DefaultCompanyId { get; set; }

        public string StateName { get; set; }
        public string SpocName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public byte? NoGst { get; set; }
    }
    public class UserRegisterDTO
    {
        public string Name { get; set; }
        public string Mobile { get; set; }

        public string Email { get; set; }
        public string Company { get; set; }

    }
}
