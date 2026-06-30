using BAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace BAL.DAL
{
    public class OtpDAL
    {
        public OtpDAL() { }
        public bool Add(OtpDTO otp)
        {
            var sql = new SQL();
            try
            {

                if (String.IsNullOrEmpty(otp.Email) && String.IsNullOrEmpty(otp.MobileNo))
                {
                    throw new Exception("Please provide either email or mobile no");
                }
                if (!String.IsNullOrEmpty(otp.Email) && String.IsNullOrEmpty(otp.EmailOTP))
                {
                    throw new Exception("Please provide email and otp");
                }
                if (!String.IsNullOrEmpty(otp.MobileNo) && String.IsNullOrEmpty(otp.MobileNo))
                {
                    throw new Exception("Please provide mobile and otp");
                }
                sql.AddParameter("@mobileOTP", DbType.String, ParameterDirection.Input, 0, otp.MobileOTP);
                sql.AddParameter("@emailOTP", DbType.String, ParameterDirection.Input, 0, otp.EmailOTP);
                sql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, otp.CreatedOn);
                sql.AddParameter("@ValidTill", DbType.DateTime, ParameterDirection.Input, 0, otp.ValidTill);
                sql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, otp.Guid);
                sql.AddParameter("@mobileNo", DbType.String, ParameterDirection.Input, 0, otp.MobileNo);
                sql.AddParameter("@email", DbType.String, ParameterDirection.Input, 0, otp.Email);
                sql.ExecuteNonQuery(ADD);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(string otpType, long otpId)
        {
            var sql = new SQL();
            try
            {

                sql.AddParameter("@otpType", DbType.String, ParameterDirection.Input, 0, otpType);
                sql.AddParameter("@otpId", DbType.String, ParameterDirection.Input, 0, otpId);

                sql.ExecuteNonQuery(UPDATE_OTP);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public OtpDTO GetByOtp(string otp)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@otp", DbType.String, ParameterDirection.Input, 0, otp);
                var ds = sql.ExecuteDataSet(GET_BYOTP);
                var dto = (from d in ds.Tables[0].AsEnumerable()
                           select new OtpDTO
                           {
                               MobileNo = d["MobileNo"] == DBNull.Value ? null : d.Field<string>("MobileNo"),
                               Email = d["Email"] == DBNull.Value ? null : d.Field<string>("Email"),
                               MobileOTP = d["MobileOTP"] == DBNull.Value ? null : d.Field<string>("MobileOTP"),
                               EmailOTP = d["EmailOTP"] == DBNull.Value ? null : d.Field<string>("EmailOTP"),
                               MobileOTPVerified = d["MobileOTPVerified"] == DBNull.Value ? false : d.Field<bool>("MobileOTPVerified"),
                               EmailOTPVerified = d["EmailOTPVerified"] == DBNull.Value ? false : d.Field<bool>("EmailOTPVerified"),
                               CreatedOn = d.Field<DateTime>("CreatedOn"),
                               ValidTill = d.Field<DateTime>("ValidTill"),
                               Guid = d.Field<string>("GuId"),
                               OtpId = d.Field<long>("OtpId"),

                           }
                           ).ToList();
                return dto.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public OtpDTO GetOTPByGuId(string guId)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, guId);
                var ds = sql.ExecuteDataSet(GET_OTPBYGUID);
                var dto = (from d in ds.Tables[0].AsEnumerable()
                           select new OtpDTO
                           {
                               MobileNo = d["MobileNo"] == DBNull.Value ? null : d.Field<string>("MobileNo"),
                               Email = d["Email"] == DBNull.Value ? null : d.Field<string>("Email"),
                               MobileOTP = d["MobileOTP"] == DBNull.Value ? null : d.Field<string>("MobileOTP"),
                               EmailOTP = d["EmailOTP"] == DBNull.Value ? null : d.Field<string>("EmailOTP"),
                               MobileOTPVerified = d["MobileOTPVerified"] == DBNull.Value ? false : d.Field<bool>("MobileOTPVerified"),
                               EmailOTPVerified = d["EmailOTPVerified"] == DBNull.Value ? false : d.Field<bool>("EmailOTPVerified"),
                               CreatedOn = d.Field<DateTime>("CreatedOn"),
                               ValidTill = d.Field<DateTime>("ValidTill"),
                               Guid = d.Field<string>("GuId"),
                               OtpId = d.Field<long>("OtpId"),

                           }
                           ).ToList();
                return dto.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region procedures
        const string ADD = "p_otp_ins";
        const string GET_BYOTP = "p_otp_sel";
        const string UPDATE_OTP = "p_otp_upd";
        const string GET_OTPBYGUID = "p_otpByGuid_sel";


        #endregion
    }
}
