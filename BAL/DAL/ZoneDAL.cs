using BAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace BAL.DAL
{
    internal class ZoneDAL
    {
        public ZoneDAL() { }

        public async Task<bool> Save(ZonesDTO dto)
        {
            var sql = new SQL();

            try
            {
                sql.BeginTransaction();
                sql.NewCommand();

                sql.AddParameter("@name", DbType.String, ParameterDirection.Input, 0, dto.Name);
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                if (dto.ZoneId == 0)
                {
                    sql.AddParameter("@createdBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
                    sql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedOn);
                    sql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, dto.GuId);
                    dto.ZoneId = Convert.ToInt32(await sql.ExecuteScalarAsync("p_zone_ins"));
                }
                else
                {
                    sql.AddParameter("@zoneId", DbType.Int32, ParameterDirection.Input, 0, dto.ZoneId);
                    sql.AddParameter("@ModifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);
                    sql.AddParameter("@ModifiedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);

                    var s = await sql.ExecuteNonQueryAsync("p_zone_upd");
                }


                if (dto.ZoneId == 0)
                {
                    throw new Exception("Could not create Zone ");
                }



                foreach (var loc in dto.Localities)
                {
                    sql.NewCommand();

                  
                 

                    sql.AddParameter("@name", DbType.String, ParameterDirection.Input, 0, loc.Name);
                    sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                    sql.AddParameter("@zoneId", DbType.Int32, ParameterDirection.Input, 0, dto.ZoneId);
                    if (loc.LocalityId == 0)
                    {
                        sql.AddParameter("@createdBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
                        sql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedOn);
                        sql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, dto.GuId);
                        var s = await sql.ExecuteNonQueryAsync("p_localities_ins");
                    }
                    else
                    {
                        sql.AddParameter("@ModifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);
                        sql.AddParameter("@ModifiedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);
                        sql.AddParameter("@localityId", DbType.Int32, ParameterDirection.Input, 0, loc.LocalityId);
                        sql.AddParameter("@deleted", DbType.Int32, ParameterDirection.Input, 0, loc.Deleted);

                        var x = await sql.ExecuteNonQueryAsync("p_localities_upd");
                    }


                }
                sql.Commit();

            }
            catch (Exception ex)
            {
                sql.Rollback();
                throw ex;
            }
            return true;
        }

        public async Task<IEnumerable<ZonesDTO>> ZonesList(ZonesDTO dto)
        {
            var sql = new SQL();

            try
            {
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                return await sql.QueryAsync<ZonesDTO>("p_zone_list");
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public async Task<ZonesDTO> ZoneById(ZonesDTO dto)
        {
            var sql = new SQL();

            try
            {

                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                sql.AddParameter("@zoneId", DbType.Int32, ParameterDirection.Input, 0, dto.ZoneId);

                var data = await sql.QueryMultipleAsync("p_zone_byId");
                if (data != null)
                {
                    dto = await data.ReadFirstAsync<ZonesDTO>();
                    dto.Localities = (await data.ReadAsync<LocalityDTO>()).ToList();
                }
                return dto;
            }
            catch (Exception ex)
            {
                sql.Rollback();
                throw ex;
            }

        }
        public async Task<int> DeleteZone(ZonesDTO dto)
        {
            var sql = new SQL();

            try
            {
                sql.BeginTransaction();
                sql.NewCommand();
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                sql.AddParameter("@zoneId", DbType.Int32, ParameterDirection.Input, 0, dto.ZoneId);
                sql.AddParameter("@ModifiedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);
                sql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);


                var x = await sql.ExecuteNonQueryAsync("p_zone_del");
                sql.Commit();
                return x;
            }
            catch (Exception ex)
            {
                sql.Rollback();
                throw ex;
            }

        }
    }
}
