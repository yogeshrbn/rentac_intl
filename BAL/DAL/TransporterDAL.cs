using BAL.DTO;
using BAL.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DAL
{
    public class TransporterDAL
    {

        public bool Save(TransporterDTO dto)
        {

            try
            {
                SQL objSql = new SQL();
                objSql.AddParameter("@name", DbType.String, ParameterDirection.Input, 0, dto.Name);
                objSql.AddParameter("@gst", DbType.String, ParameterDirection.Input, 0, dto.GST);
                objSql.AddParameter("@phone", DbType.String, ParameterDirection.Input, 0, dto.Phone);
                objSql.AddParameter("@email", DbType.String, ParameterDirection.Input, 0, dto.Email);

                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                if (dto.TransporterId == 0)
                {
                    objSql.AddParameter("@isActive", DbType.Byte, ParameterDirection.Input, 0, dto.IsActive);
                    objSql.AddParameter("@createdBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
                    objSql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedOn);

                    return objSql.ExecuteNonQuery(ADD) > 0;
                }
                else
                {
                    objSql.AddParameter("@transporterId", DbType.Int32, ParameterDirection.Input, 0, dto.TransporterId);
                    return objSql.ExecuteNonQuery(UPDATE) > 0;
                }
            }
            catch (Exception ex) { throw ex; }

        }
        public List<TransporterDTO> GetAll(TransporterDTO dto)
        {
            SQL objSql = new SQL();
            try
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                return objSql.ContructList<TransporterDTO>(objSql.ExecuteDataSet(SELECTALL));
            }
            catch (Exception ex) { throw ex; }
        }
        #region Procedures
        const string ADD = "p_transporter_ins";
        const string UPDATE = "p_transporter_upd";
        const string SELECTALL = "p_transporter_all";
        const string SELECTBYID = "p_transporter_byId";

        #endregion
    }
}
