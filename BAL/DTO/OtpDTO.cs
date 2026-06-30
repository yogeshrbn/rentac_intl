using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class OtpDTO
    {
        public OtpDTO() { }

        public long OtpId { get; set; }
        public string MobileOTP { get; set; }
        public string EmailOTP { get; set; }
        public bool MobileOTPVerified { get; set;   }
        public bool EmailOTPVerified { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ValidTill { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Guid { get; set; }

    }


}
