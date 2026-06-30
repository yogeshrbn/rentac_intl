using BAL.Data.Contracts;
using BAL.DTO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Data.Repository
{
    public class ProductTaxClassificationRepository : IProductTaxClassificationContract
    {
        private readonly string _connectionString;

        //public ProductTaxClassificationRepository(IConfiguration configuration)
        //{
        //    _connectionString = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString
        //        ?? throw new ArgumentNullException("DefaultConnection string is missing");
        //}

        public ProductTaxClassificationRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString
                ?? throw new ArgumentNullException("DefaultConnection string is missing");
        }
        public async Task<long> InsertAsync(SaveProductTaxClassificationDto dto)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ProductTaxClassification_Insert", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CompanyId", dto.CompanyId);
                    cmd.Parameters.AddWithValue("@ProductId", dto.ProductId);
                    cmd.Parameters.AddWithValue("@TransactionType", dto.TransactionType);
                    cmd.Parameters.AddWithValue("@TaxCategoryId", dto.TaxCategoryId);

                    await con.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();

                    return Convert.ToInt64(result);
                }
            }
        }

        public async Task UpsertAsync(SaveProductTaxClassificationDto dto)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ProductTaxClassfication_Upsert", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@CompanyId", dto.CompanyId);
                    cmd.Parameters.AddWithValue("@ProductId", dto.ProductId);
                    cmd.Parameters.AddWithValue("@TransactionType", dto.TransactionType);
                    cmd.Parameters.AddWithValue("@TaxCategoryId", dto.TaxCategoryId);
                    cmd.Parameters.AddWithValue("@Nature", dto.Nature);
                    cmd.Parameters.AddWithValue("@hsnCode", dto.HsnCode);
                    cmd.Parameters.AddWithValue("@sacCode", dto.SacCode);
                    cmd.Parameters.AddWithValue("@isReverseCharge", dto.IsReverseCharge);
                    cmd.Parameters.AddWithValue("@isExempt", dto.IsExempt);
                    cmd.Parameters.AddWithValue("@isNillRated", dto.IsNillRated);
                    cmd.Parameters.AddWithValue("@isZeroRated", dto.IsZeroRated);
                   

                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> UpdateAsync(long taxClassificationId, int taxCategoryId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ProductTaxClassification_Update", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TaxClassificationId", taxClassificationId);
                    cmd.Parameters.AddWithValue("@TaxCategoryId", taxCategoryId);

                    await con.OpenAsync();
                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> DeleteAsync(long taxClassificationId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ProductTaxClassification_Delete", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TaxClassificationId", taxClassificationId);

                    await con.OpenAsync();
                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<ProductTaxClassificationDto>> GetByProductAsync(int companyId, int productId)
        {
            var list = new List<ProductTaxClassificationDto>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ProductTaxClassfication_GetByProductId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    cmd.Parameters.AddWithValue("@ProductId", productId);

                    await con.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new ProductTaxClassificationDto
                            {
                                TaxClassificationId = Convert.ToInt64(reader["TaxClassificationId"]),
                                CompanyId = Convert.ToInt32(reader["CompanyId"]),
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                TransactionType = Convert.ToString(reader["TransactionType"]),
                                TaxCategoryId = Convert.ToInt32(reader["TaxCategoryId"]),
                                Nature = Convert.ToString(reader["Nature"]),
                                IsReverseCharge = Convert.ToBoolean(reader["IsReverseCharge"]),
                                IsExempt = Convert.ToBoolean(reader["IsExempt"]),
                                IsNillRated = Convert.ToBoolean(reader["IsNillRated"]),
                                IsZeroRated = Convert.ToBoolean(reader["IsZeroRated"]),
                                SacCode = Convert.ToString(reader["SacCode"]),
                                HsnCode = Convert.ToString(reader["HsnCode"]),
                            });
                        }
                        return list;
                    }
                }
            }
        }

        public async Task<ProductTaxClassificationDto> GetByTransactionAsync(int companyId, int productId, string transactionType)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ProductTaxClassification_GetByTransaction", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    cmd.Parameters.AddWithValue("@TransactionType", transactionType);

                    await con.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new ProductTaxClassificationDto
                            {
                                TaxClassificationId = Convert.ToInt64(reader["TaxClassificationId"]),
                                CompanyId = companyId,
                                ProductId = productId,
                                TransactionType = transactionType,
                                TaxCategoryId = Convert.ToInt32(reader["TaxCategoryId"])
                            };
                        }
                        return null;
                    }
                }
            }
        }
    }
}
