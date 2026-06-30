using BAL.DTO;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BAL.DAL
{
    internal class RoleCostingDAL
    {
        public async Task<IEnumerable<RoleCostingDTO>> List(int companyId)
        {
            using (var sql = new SQL())
            {
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
                return await sql.QueryAsync<RoleCostingDTO>(ROLE_COSTING_LIST);
            }
        }

        public async Task<RoleCostingDTO> ById(int roleCostingId, int companyId)
        {
            using (var sql = new SQL())
            {
                sql.AddParameter("@roleCostingId", DbType.Int32, ParameterDirection.Input, 0, roleCostingId);
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
                return await sql.QueryFirstAsync<RoleCostingDTO>(ROLE_COSTING_BYID);
            }
        }

        public async Task<int> Save(RoleCostingDTO dto)
        {
            using (var sql = new SQL())
            {
                sql.AddParameter("@roleId", DbType.Byte, ParameterDirection.Input, 0, dto.RoleId);
                sql.AddParameter("@perHourCost", DbType.Decimal, ParameterDirection.Input, 0, dto.PerHourCost);
                sql.AddParameter("@perDayCost", DbType.Decimal, ParameterDirection.Input, 0, dto.PerDayCost);
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);

                if (dto.RoleCostingId > 0)
                {
                    sql.AddParameter("@roleCostingId", DbType.Int32, ParameterDirection.Input, 0, dto.RoleCostingId);
                    sql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);
                    sql.AddParameter("@modifiedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);
                    return await sql.ExecuteNonQueryAsync(ROLE_COSTING_UPD);
                }

                sql.AddParameter("@createdBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
                sql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedOn);
                sql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, dto.GuId);
                return await sql.ExecuteNonQueryAsync(ROLE_COSTING_INS);
            }
        }

        public async Task<int> Delete(RoleCostingDTO dto)
        {
            using (var sql = new SQL())
            {
                sql.AddParameter("@roleCostingId", DbType.Int32, ParameterDirection.Input, 0, dto.RoleCostingId);
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                sql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);
                sql.AddParameter("@modifiedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);
                return await sql.ExecuteNonQueryAsync(ROLE_COSTING_DEL);
            }
        }

        const string ROLE_COSTING_LIST = "p_roleCosting_list";
        const string ROLE_COSTING_BYID = "p_roleCosting_byId";
        const string ROLE_COSTING_INS = "p_roleCosting_ins";
        const string ROLE_COSTING_UPD = "p_roleCosting_upd";
        const string ROLE_COSTING_DEL = "p_roleCosting_del";
    }
}
