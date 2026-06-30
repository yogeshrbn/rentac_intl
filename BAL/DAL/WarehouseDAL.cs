using BAL.Objects;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;

namespace BAL.DAL
{
    public class WarehouseDAL
    {
        public async Task<int> CreateWarehouse(WarehouseDTO dto)
        {
            using (var connection = new SQL())
            {

                connection.AddParameter("@Name", DbType.String, ParameterDirection.Input, 0, dto.Name);
                connection.AddParameter("@Location", DbType.String, ParameterDirection.Input, 0, dto.Location);
                connection.AddParameter("@Capacity", DbType.Int32, ParameterDirection.Input, 0, dto.Capacity);
                connection.AddParameter("@CreatedBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
                connection.AddParameter("@RbnClientId", DbType.Int32, ParameterDirection.Input, 0, dto.RbnClientId);
                connection.AddParameter("@StatusId", DbType.Byte, ParameterDirection.Input, 0, dto.StatusId);
                connection.AddParameter("@CreatedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedOn);

                dto.WarehouseId = Convert.ToInt32(await connection.ExecuteScalarAsync("p_Warehouse_Insert"));
                return dto.WarehouseId;
            }
        }
        public async Task<WarehouseDTO> UpdateWarehouse(WarehouseDTO dto)
        {
            using (var connection = new SQL())
            {

                connection.AddParameter("@Name", DbType.String, ParameterDirection.Input, 0, dto.Name);
                connection.AddParameter("@Location", DbType.String, ParameterDirection.Input, 0, dto.Location);
                connection.AddParameter("@Capacity", DbType.Int32, ParameterDirection.Input, 0, dto.Capacity);
                connection.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);
                connection.AddParameter("@RbnClientId", DbType.Int32, ParameterDirection.Input, 0, dto.RbnClientId);

                connection.AddParameter("@modifiedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);
                connection.AddParameter("@warehouseId", DbType.Int32, ParameterDirection.Input, 0, dto.WarehouseId);

                return await connection.QueryFirstAsync<WarehouseDTO>("p_Warehouse_Update");

            }
        }

        public async Task<IEnumerable<WarehouseDTO>> GetAll(WarehouseDTO dto)
        {
            using (var connection = new SQL())
            {
 
                connection.AddParameter("@RbnClientId", DbType.Int32, ParameterDirection.Input, 0, dto.RbnClientId);
                 

                return await connection.QueryAsync<WarehouseDTO>("p_Warehouse_GetAll");

            }
        }

        public async Task<WarehouseDTO> GetById(WarehouseDTO dto)
        {
            using (var connection = new SQL())
            {

                connection.AddParameter("@RbnClientId", DbType.Int32, ParameterDirection.Input, 0, dto.RbnClientId);
                connection.AddParameter("@warehouseId", DbType.Int32, ParameterDirection.Input, 0, dto.WarehouseId);


                return await connection.QueryFirstAsync<WarehouseDTO>("p_Warehouse_GetById");

            }
        }
        public async Task<WarehouseDTO> UpdateStatus(WarehouseDTO dto)
        {
            using (var connection = new SQL())
            {

                connection.AddParameter("@RbnClientId", DbType.Int32, ParameterDirection.Input, 0, dto.RbnClientId);
                connection.AddParameter("@warehouseId", DbType.Int32, ParameterDirection.Input, 0, dto.WarehouseId);
                connection.AddParameter("@statusId", DbType.Byte, ParameterDirection.Input, 0, dto.StatusId);


                return await connection.QueryFirstAsync<WarehouseDTO>("p_Warehouse_UpdateStatus");

            }
        }
    }
}
