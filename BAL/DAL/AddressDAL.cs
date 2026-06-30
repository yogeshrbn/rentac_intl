using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using System.Data;
using BAL.Enums;
namespace BAL.DAL
{
    internal class AddressDAL
    {
        public bool Save(SQL objSql, AddressDTO dto)
        {
            //SQL objSql = new SQL();
            if (objSql == null)
            {
                objSql = new SQL();
            }

            objSql.AddParameter("@Address1", DbType.String, ParameterDirection.Input, 0, dto.Address1);
            objSql.AddParameter("@Address2", DbType.String, ParameterDirection.Input, 0, dto.Address2);
            objSql.AddParameter("@City", DbType.String, ParameterDirection.Input, 0, dto.City);
            objSql.AddParameter("@State", DbType.String, ParameterDirection.Input, 0, dto.State);
            objSql.AddParameter("@Email", DbType.String, ParameterDirection.Input, 0, dto.Email);
            objSql.AddParameter("@Phone1", DbType.String, ParameterDirection.Input, 0, dto.Phone1);
            objSql.AddParameter("@Phone2", DbType.String, ParameterDirection.Input, 0, dto.Phone2);
            objSql.AddParameter("@Fax", DbType.String, ParameterDirection.Input, 0, dto.Fax);
            objSql.AddParameter("@Web", DbType.String, ParameterDirection.Input, 0, dto.Web);
            objSql.AddParameter("@StateId", DbType.Int32, ParameterDirection.Input, 0, dto.StateId);
            objSql.AddParameter("@ZipCode", DbType.String, ParameterDirection.Input, 0, dto.ZipCode);

            bool result;
            if (dto.AddressId == 0)
            {
                objSql.AddParameter("@AddressTypeId", DbType.Int16, ParameterDirection.Input, 0, dto.AddressTypeId);
                objSql.AddParameter("@AddressHolderId", DbType.Int32, ParameterDirection.Input, 0, dto.AddressHolderId);
                objSql.AddParameter("@RoleId", DbType.Int16, ParameterDirection.Input, 0, dto.RoleId);
                result = objSql.ExecuteNonQuery(ADD) > 0;
            }
            else
            {
                objSql.AddParameter("@AddressId", DbType.Int32, ParameterDirection.Input, 0, dto.AddressId);
                result = objSql.ExecuteNonQuery(UPDATE) > 0;
            }
            return result;
        }

        public List<AddressDTO> GetAddresses(AddressRoleType roleType, int hoderId)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@RoleId", DbType.Single, ParameterDirection.Input, 0, Convert.ToInt16(roleType));
            objSql.AddParameter("@AddressHolderId", DbType.Int32, ParameterDirection.Input, 0, hoderId);
            return objSql.ContructList<AddressDTO>(objSql.ExecuteDataSet(SELECT));

        }
        #region procedures
        const string ADD = "p_Address_ins";
        const string UPDATE = "p_Address_upd";
        const string SELECT = "p_Address_sel";
        #endregion
    }
}
