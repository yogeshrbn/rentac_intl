using BAL.DAL;
using BAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace BAL.Objects
{
    public class OTP
    {
        public OTP() { }

        public bool Add(OtpDTO otp)
        {
            var otpDAL = new OtpDAL();
            return otpDAL.Add(otp);
        }

        public bool Update(string otpType, long otpId)
        {
            var otpDAL = new OtpDAL();
            return otpDAL.Update(otpType, otpId);
        }

        public OtpDTO GetByOtp(string otp)
        {
            var otpDAL = new OtpDAL();
            return otpDAL.GetByOtp(otp);
        }
        public OtpDTO GetOTPByGuId(string guId)
        {
            var otpDAL = new OtpDAL();
            return otpDAL.GetOTPByGuId(guId);
        }

    }
}
