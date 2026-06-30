using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.Enums;
using BAL.DTO;
using System.Data;
using BAL.Exceptions;
using System.Data.SqlClient;
namespace BAL.DAL
{
    internal class VehicleDAL
    {
        internal bool Add(VehicleDTO dto)
        {
            try
            {
                SQL sql = new SQL();
                sql.AddParameter("@Name", DbType.String, ParameterDirection.Input, 0, dto.Name);

                sql.AddParameter("@RegNumber", DbType.String, ParameterDirection.Input, 0, dto.RegNumber);
                sql.AddParameter("@ChachisNumber", DbType.String, ParameterDirection.Input, 0, dto.ChachisNumber);
                sql.AddParameter("@EngineNumber", DbType.String, ParameterDirection.Input, 0, dto.EngineNumber);
                sql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                if (dto.VehicleId > 0)
                {
                    sql.AddParameter("@VehicleId", DbType.Int32, ParameterDirection.Input, 0, dto.VehicleId);
                    sql.ExecuteNonQuery(UPDATE);
                }
                else
                {
                  
                    dto.VehicleId = Convert.ToInt32(sql.ExecuteScalar(ADD));
                }
                return true;
            }
            catch (SqlException ex)
            {
                throw new UDFException(ex.Message,ex.ErrorCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<VehicleDTO> GetAll(int companyId)
        {
            SQL sql = new SQL();
            sql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            return sql.ContructList<VehicleDTO>(sql.ExecuteDataSet(GET_ALL));
        }
        public VehicleDTO GetById(int vehicleId)
        {
            SQL sql = new SQL();
            sql.AddParameter("@vehicleId", DbType.Int32, ParameterDirection.Input, 0, vehicleId);
            return sql.ContructList<VehicleDTO>(sql.ExecuteDataSet(GET_BY_ID)).FirstOrDefault();
        }
        #region Procs
        const string ADD = "p_Vehicle_ins";
        const string UPDATE = "p_Vehicle_upd";
        const string GET_ALL = "p_Vehicle_sellAll";
        const string GET_BY_ID = "p_Vehicle_sellById";
        #endregion
    }
}
