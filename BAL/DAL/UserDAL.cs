using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using BAL.DTO;
using System.Data.SqlClient;

namespace BAL.DAL
{
    internal class UserDAL
    {

        internal bool SetDefaultCompany(int userId, int companyId)
        {
            SQL sql = new SQL();
            sql.AddParameter("@UserId", DbType.Int16, ParameterDirection.Input, 0, userId);
            sql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, companyId);
            return sql.ExecuteNonQuery(SET_DEFAULT_COMPANY) > 0;

        }
        internal List<UserDTO> GetAllUsers(int rbnClientId)
        {
            SQL sql = new SQL();

            sql.AddParameter("@rbnClientId", DbType.Int16, ParameterDirection.Input, 0, rbnClientId);
            return sql.ContructList<UserDTO>(sql.ExecuteDataSet(GET_ALL_USERS));

        }
        public bool CreateUser(UserDTO dto)
        {
            try
            {
                SQL sql = new SQL();
                sql.AddParameter("@rbnClientId", DbType.Int16, ParameterDirection.Input, 0, dto.RbnClientId);
                sql.AddParameter("@UserName", DbType.String, ParameterDirection.Input, 0, dto.Phone);
                sql.AddParameter("@Password", DbType.String, ParameterDirection.Input, 0, dto.Password);
                sql.AddParameter("@Email", DbType.String, ParameterDirection.Input, 0, dto.Email);
                sql.AddParameter("@Phone", DbType.String, ParameterDirection.Input, 0, dto.Phone);
                sql.AddParameter("@FullName", DbType.String, ParameterDirection.Input, 0, dto.FullName);
                sql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, dto.DefaultCompanyId);
                sql.AddParameter("@RoleId", DbType.Int16, ParameterDirection.Input, 0, dto.RoleId);
                sql.AddParameter("@active", DbType.Boolean, ParameterDirection.Input, 0, dto.Active);
                sql.AddParameter("@companies", DbType.String, ParameterDirection.Input, 0, dto.Companies);
                sql.AddParameter("@AllowSwitchCompany", DbType.Boolean, ParameterDirection.Input, 0, dto.AllowSwitchCompany);
                return sql.ExecuteNonQuery(CREATE_USER) > 0;

            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateUser(UserDTO dto)
        {
            SQL sql = new SQL();
            sql.AddParameter("@UserId", DbType.Int16, ParameterDirection.Input, 0, dto.UserId);

            sql.AddParameter("@Email", DbType.String, ParameterDirection.Input, 0, dto.Email);
            sql.AddParameter("@Phone", DbType.String, ParameterDirection.Input, 0, dto.Phone);
            sql.AddParameter("@FullName", DbType.String, ParameterDirection.Input, 0, dto.FullName);
            sql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, dto.DefaultCompanyId);
            sql.AddParameter("@RoleId", DbType.Int16, ParameterDirection.Input, 0, dto.RoleId);
            sql.AddParameter("@companies", DbType.String, ParameterDirection.Input, 0, dto.Companies);

            sql.AddParameter("@AllowSwitchCompany", DbType.Boolean, ParameterDirection.Input, 0, dto.AllowSwitchCompany);
            return sql.ExecuteNonQuery(UPDATE_USER) > 0;
        }
        public bool ResetPassword(UserDTO dto)
        {
            SQL sql = new SQL();
            sql.AddParameter("@UserId", DbType.Int16, ParameterDirection.Input, 0, dto.UserId);
            sql.AddParameter("@Password", DbType.String, ParameterDirection.Input, 0, dto.Password);

            return sql.ExecuteNonQuery(RESET_PASSWORD) > 0;
        }
        public bool UpdateStatus(int userId, bool status)
        {
            SQL sql = new SQL();
            sql.AddParameter("@UserId", DbType.Int16, ParameterDirection.Input, 0, userId);
            sql.AddParameter("@Active", DbType.Boolean, ParameterDirection.Input, 0, status);
            return sql.ExecuteNonQuery(DEACTIVATE_USER) > 0;
        }
        public FunctionDTO GetRouteAccess(int userId, string route)
        {
            SQL sql = new SQL();
            sql.AddParameter("@UserId", DbType.Int16, ParameterDirection.Input, 0, userId);
            sql.AddParameter("@route", DbType.String, ParameterDirection.Input, 0, route);

            return sql.ContructList<FunctionDTO>(sql.ExecuteDataSet(GET_ROUTE_ACCESS)).FirstOrDefault();
        }
        public List<FunctionDTO> GetAllowedRoutes(int userId)
        {
            SQL sql = new SQL();
            sql.AddParameter("@UserId", DbType.Int16, ParameterDirection.Input, 0, userId);


            return sql.ContructList<FunctionDTO>(sql.ExecuteDataSet(ALLOWED_ROUTES));
        }
        internal UserDTO GetById(int userId, int finYearId)
        {
            SQL sql = new SQL();

            sql.AddParameter("@userId", DbType.Int16, ParameterDirection.Input, 0, userId);
            sql.AddParameter("@finYear", DbType.Int16, ParameterDirection.Input, 0, finYearId);
            return sql.ContructList<UserDTO>(sql.ExecuteDataSet(GET_USER_BYID)).FirstOrDefault();

        }
        public bool UpdateProfilePic(int userId, string picPath)
        {

            SQL sql = new SQL();

            sql.AddParameter("@userId", DbType.Int16, ParameterDirection.Input, 0, userId);
            sql.AddParameter("@profilePicPath", DbType.String, ParameterDirection.Input, 0, picPath);
            return sql.ExecuteNonQuery(UPDATE_PROFILE_PIC) > 0;
        }
        public bool ChangePassword(int userId, string currentPassword, string newPassword)

        {
            SQL sql = new SQL();

            sql.AddParameter("@userId", DbType.Int32, ParameterDirection.Input, 0, userId);
            sql.AddParameter("@currentPassword", DbType.String, ParameterDirection.Input, 0, currentPassword);
            sql.AddParameter("@newPassword", DbType.String, ParameterDirection.Input, 0, newPassword);

            return sql.ExecuteNonQuery(CHANGE_PWD) > 0;
        }
        public bool CreateVarificationLink(VerificationLinkDTO lnk)

        {
            SQL sql = new SQL();

            sql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, lnk.GuId);
            sql.AddParameter("@email", DbType.String, ParameterDirection.Input, 0, lnk.Email);
            sql.AddParameter("@createdAt", DbType.DateTime, ParameterDirection.Input, 0, lnk.CreatedAt);
            sql.AddParameter("@validTill", DbType.DateTime, ParameterDirection.Input, 0, lnk.ValidTill);

            return sql.ExecuteNonQuery(SEND_VERIFICATIONLINK) > 0;
        }
        public VerificationLinkDTO GetVarificationLink(string guId)

        {
            SQL sql = new SQL();

            sql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, guId);
            var ds = sql.ExecuteDataSet(GET_VERIFICATIONLINK);
            var dto = (from p in ds.Tables[0].AsEnumerable()
                       select new VerificationLinkDTO
                       {
                           GuId = p.Field<string>("GuId"),
                           Email = p.Field<string>("Email"),
                           ValidTill = p.Field<DateTime>("ValidTill"),
                           CreatedAt = p.Field<DateTime>("CreatedAt"),
                           Used = p.Field<byte>("used"),
                       }).FirstOrDefault();


            return dto;
        }
        public UserDTO GetByEmail(string emmail)

        {
            SQL sql = new SQL();

            sql.AddParameter("@email", DbType.String, ParameterDirection.Input, 0, emmail);
            var ds = sql.ExecuteDataSet(GET_BYEMAIL);
            var dto = (from p in ds.Tables[0].AsEnumerable()
                       select new UserDTO
                       {

                           Email = p.Field<string>("Email"),
                           UserId = p.Field<int>("UserId")


                       }).FirstOrDefault();


            return dto;
        }
        public UserDTO GetByPhone(string phone)

        {
            SQL sql = new SQL();

            sql.AddParameter("@phone", DbType.String, ParameterDirection.Input, 0, phone);
            var ds = sql.ExecuteDataSet(GET_BYPHONE);
            var dto = (from p in ds.Tables[0].AsEnumerable()
                       select new UserDTO
                       {
                           UserName = p.Field<string>("UserName"),

                           Phone = p.Field<string>("Phone"),
                           UserId = p.Field<int>("UserId"),
                           Password = p.Field<string>("Password")

                       }).FirstOrDefault();
            return dto;

        }
        #region Procedures
        const string SET_DEFAULT_COMPANY = "p_users_defaultCompay";
        const string GET_ALL_USERS = "p_getAll_users";
        const string CREATE_USER = "p_CreateUser";
        const string UPDATE_USER = "p_updateUser";
        const string RESET_PASSWORD = "p_resetPwd";
        const string DEACTIVATE_USER = "p_user_UpdateStatus";
        const string GET_ROUTE_ACCESS = "p_GetRouteAccess";
        const string GET_USER_BYID = "p_userById";
        const string UPDATE_PROFILE_PIC = "p_updateProfilePic";
        const string ALLOWED_ROUTES = "p_GetAllowedRoutes";
        const string CHANGE_PWD = "p_changePassword";
        const string SEND_VERIFICATIONLINK = "p_verificationLink_ins";
        const string GET_VERIFICATIONLINK = "p_verificationLink_sel";
        const string GET_BYEMAIL = "p_userByemail";
        const string GET_BYPHONE = "p_userByphone";


        #endregion
    }
}
