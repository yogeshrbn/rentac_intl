using BAL.Contracts.Repository;
using BAL.DAL;
using BAL.DTO;
using BAL.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
//using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace BAL.Data.Repository
{
    public class ProductionRepository : IProductionRepository
    {
        private readonly string _connectionString;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ProductionRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString
                ?? throw new ArgumentNullException("DefaultConnection string is missing");

        }

        public async Task<Production> GetByIdAsync(long productionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {

                var parameters = new DynamicParameters();
                parameters.Add("@ProductionId", productionId);

                var result = await connection.QueryFirstOrDefaultAsync<Production>(
                    "usp_Production_GetById",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        public async Task<Production> GetByGuidAsync(string guid)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@GuId", guid);

                var result = await connection.QueryFirstOrDefaultAsync<Production>(
                    "usp_Production_GetByGuid",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        public async Task<PagedResultDto<Production>> ListAsync(ProductionQueryDto query)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CompanyId", query.CompanyId);
                parameters.Add("@ProductId", query.ProductId);
                parameters.Add("@ClientId", query.ClientId);
                parameters.Add("@StatusId", query.StatusId);
                parameters.Add("@StartDate", query.StartDate?.ToString("yyyy-MM-dd"));
                parameters.Add("@EndDate", query.EndDate?.ToString("yyyy-MM-dd"));
                parameters.Add("@PageNumber", query.PageNumber);
                parameters.Add("@PageSize", query.PageSize);
                parameters.Add("@SortColumn", query.SortColumn);
                parameters.Add("@SortDirection", query.SortDirection);
                parameters.Add("@TotalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var results = await connection.QueryAsync<Production>(
                    "usp_Production_List",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                var totalRecords = parameters.Get<int>("@TotalRecords");

                return new PagedResultDto<Production>
                {
                    Data = results.ToList(),
                    TotalRecords = totalRecords,
                    PageNumber = query.PageNumber,
                    PageSize = query.PageSize
                };
            }
        }

        public async Task<long> InsertAsync(Production production)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                var txn = connection.BeginTransaction();
                try
                {

                    var parameters = new DynamicParameters();
                    parameters.Add("@ProductId", production.ProductId);
                    parameters.Add("@Quantity", production.Quantity);
                    parameters.Add("@StatusId", production.StatusId);
                    parameters.Add("@CompanyId", production.CompanyId);
                    parameters.Add("@ClientId", production.ClientId);
                    parameters.Add("@SaleOrderNo", production.SaleOrderNo);
                    parameters.Add("@Description", production.Description);
                    parameters.Add("@PlannedStartDate", production.PlannedStartDate.ToString("yyyy-MM-dd"));
                    parameters.Add("@ActualStartDate", production.ActualStartDate.ToString("yyyy-MM-dd"));
                    parameters.Add("@ActualEndDate", production.ActualEndDate.ToString("yyyy-MM-dd"));
                    parameters.Add("@PlannedEndDate", production.PlannedEndDate.ToString("yyyy-MM-dd"));
                    parameters.Add("@CreatedBy", production.CreatedBy);
                    parameters.Add("@NewProductionId", dbType: DbType.Int64, direction: ParameterDirection.Output);
                    parameters.Add("@GuId", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync(
                        "p_Production_Insert",
                        parameters, txn,
                        commandType: CommandType.StoredProcedure);

                    var newProductionId = parameters.Get<long>("@NewProductionId");
                    var newGuid = parameters.Get<string>("@GuId");

                    production.ProductionId = newProductionId;
                    production.GuId = newGuid;

                    foreach (var bomItem in production.BOM)
                    {
                        parameters = new DynamicParameters();
                        parameters.Add("@ProductId", bomItem.ProductId);
                        parameters.Add("@Quantity", bomItem.Quantity);
                        parameters.Add("@ProductionId", production.ProductionId);
                        parameters.Add("@Consumed", bomItem.Consumed);
                        parameters.Add("@Returned", bomItem.Returned);
                        parameters.Add("@CreatedBy", production.CreatedBy);

                        parameters.Add("@ProductionBomId", dbType: DbType.Int64, direction: ParameterDirection.Output);

                        await connection.ExecuteAsync(
                        "p_ProductionBOM_Insert",
                        parameters, txn,
                        commandType: CommandType.StoredProcedure);

                        bomItem.ProductionBomId = parameters.Get<long>("@ProductionBomId");


                    }
                    foreach (var po in production.Operations)
                    {
                        parameters = new DynamicParameters();
                        parameters.Add("@OperationId", po.OperationId);
                        parameters.Add("@Quantity", po.Quantity);
                        parameters.Add("@ProductionId", production.ProductionId);
                        parameters.Add("@CompanyId", production.CompanyId);
                        parameters.Add("@CreatedOn", production.CreatedOn);
                        parameters.Add("@CreatedBy", production.CreatedBy);
                        parameters.Add("@guId", Guid.NewGuid().ToString());
                        parameters.Add("@statusId", po.StatusId);
                        parameters.Add("@ProductionOperationId", dbType: DbType.Int64, direction: ParameterDirection.Output);

                        await connection.ExecuteAsync(
                        "p_poOperation_ins",
                        parameters, txn,
                        commandType: CommandType.StoredProcedure);

                        po.ProductionOperationId = parameters.Get<long>("@ProductionOperationId");


                    }
                    txn.Commit();

                    return newProductionId;
                }
                catch (Exception ex)
                {
                    txn.Rollback();
                    throw ex;
                }
            }
        }

        public async Task<bool> UpdateAsync(Production production)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ProductionId", production.ProductionId);
                parameters.Add("@ProductId", production.ProductId);
                parameters.Add("@Quantity", production.Quantity);
                parameters.Add("@StatusId", production.StatusId);
                parameters.Add("@CompanyId", production.CompanyId);
                parameters.Add("@ClientId", production.ClientId);
                parameters.Add("@SaleOrderNo", production.SaleOrderNo);
                parameters.Add("@Description", production.Description);
                parameters.Add("@PlannedStartDate", production.PlannedStartDate.ToString("yyyy-MM-dd"));
                parameters.Add("@ActualStartDate", production.ActualStartDate.ToString("yyyy-MM-dd"));
                parameters.Add("@ActualEndDate", production.ActualEndDate.ToString("yyyy-MM-dd"));
                parameters.Add("@PlannedEndDate", production.PlannedEndDate.ToString("yyyy-MM-dd"));
                parameters.Add("@ModifiedBy", production.ModifiedBy);
                parameters.Add("@RowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "usp_Production_Update",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                var rowsAffected = parameters.Get<int>("@RowsAffected");
                return rowsAffected > 0;
            }
        }

        public async Task<bool> DeleteAsync(long productionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ProductionId", productionId);
                parameters.Add("@RowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "usp_Production_Delete",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                var rowsAffected = parameters.Get<int>("@RowsAffected");
                return rowsAffected > 0;
            }
        }

        public async Task<IEnumerable<OperationDto>> GetOperation(int companyId)
        {            
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();               
                parameters.Add("@companyId", companyId);
                return await connection.QueryAsync<OperationDto>(
                      "p_operation_list",
                      parameters,
                      commandType: CommandType.StoredProcedure);


            }
        }
    }
}
