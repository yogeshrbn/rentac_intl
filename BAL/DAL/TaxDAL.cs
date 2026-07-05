using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using BAL.DTO;
using BAL.Enums;
namespace BAL.DAL
{
    internal class TaxDAL
    {
        public List<TaxDTO> GetAllTaxes(int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            return objSql.ContructList<TaxDTO>(objSql.ExecuteDataSet(GET_ALL));
        }
        ///// <summary>
        ///// this needs to be removed later.
        ///// </summary>
        ///// <param name="item"></param>
        ///// <returns></returns>
        //public List<TaxDTO> GetTaxes(TaxItem item)
        //{
        //    List<TaxDTO> lstTaxes = null;
        //    SQL objSql = new SQL();
        //    objSql.AddParameter("@ItemId", System.Data.DbType.Int16, System.Data.ParameterDirection.Input, 0, Convert.ToInt16(item));
        //    switch (item)
        //    {
        //        case TaxItem.WorkOrder:
        //            lstTaxes = objSql.ContructList<TaxDTO>(objSql.ExecuteDataSet(APPLICABLE_TAXES));
        //            break;
        //    }
        //    return lstTaxes;
        //}

        public List<TaxDTO> GetApplicableTaxes(TaxItem item, int itemValue)
        {

            SQL objSql = new SQL();
            objSql.AddParameter("@ItemId", System.Data.DbType.Int16, System.Data.ParameterDirection.Input, 0, Convert.ToInt16(item));
            objSql.AddParameter("@ItemValue", System.Data.DbType.Int16, System.Data.ParameterDirection.Input, 0, itemValue);
            return objSql.ContructList<TaxDTO>(objSql.ExecuteDataSet(APPLICABLE_TAXES));

        }
        public bool Save(List<TaxDTO> lst)
        {
            SQL objSql = new SQL();
            objSql.BeginTransaction();

            bool result = false;
            try
            {
                foreach (TaxDTO dto in lst)
                {
                    objSql.NewCommand();
                    objSql.AddParameter("@Rate", System.Data.DbType.Double, System.Data.ParameterDirection.Input, 0, dto.Rate);

                    objSql.AddParameter("@ModifiedBy", System.Data.DbType.Int32, System.Data.ParameterDirection.Input, 0, dto.ModifiedBy);
                    if (dto.ApplicableTaxId == 0)
                    {
                        objSql.AddParameter("@TaxId", System.Data.DbType.Int16, System.Data.ParameterDirection.Input, 0, dto.TaxId);
                        objSql.AddParameter("@Item", System.Data.DbType.Int16, System.Data.ParameterDirection.Input, 0, Convert.ToInt16(dto.ItemId));
                        objSql.AddParameter("@ItemValue", System.Data.DbType.Int32, System.Data.ParameterDirection.Input, 0, dto.ItemValue);
                        objSql.ExecuteNonQuery(ADD);
                    }
                    else
                    {
                        objSql.AddParameter("@ApplicableTaxId", System.Data.DbType.Int32, System.Data.ParameterDirection.Input, 0, dto.ApplicableTaxId);
                        objSql.ExecuteNonQuery(UPDATE);
                    }
                }
                objSql.Commit();
            }
            catch (Exception ex)
            {
                objSql.Rollback();
            }
            return result;
        }
        public async Task<IEnumerable<TaxCategoryDTO>> GetAllTaxCategories()
        {
            SQL objSql = new SQL();

            var categories = (await objSql.QueryAsync<TaxCategoryDTO>(GET_ALL_TAXCATOGRIES)).ToList();
            if (categories.Count == 0)
            {
                return categories;
            }

            objSql.NewCommand();
            var allMappings = (await objSql.QueryAsync<TaxCategoryMappingDTO>(TAX_CATEGORY_MAPPING_ALL)).ToList();
            var mappingsByCategory = allMappings
                .GroupBy(m => m.TaxCategoryId)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var category in categories)
            {
                List<TaxCategoryMappingDTO> mappings;
                if (!mappingsByCategory.TryGetValue(category.TaxCategoryId, out mappings))
                {
                    mappings = new List<TaxCategoryMappingDTO>();
                }

                AttachMappings(category, mappings);
            }

            return categories;
        }

        private static void AttachMappings(TaxCategoryDTO category, List<TaxCategoryMappingDTO> mappings)
        {
            category.Mappings = mappings ?? new List<TaxCategoryMappingDTO>();
            category.TaxIds = category.Mappings.Select(m => m.TaxId).ToList();
        }

        public async Task<TaxCategoryDTO> GetTaxCategoryById(int taxCategoryId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@TaxCategoryId", DbType.Int32, ParameterDirection.Input, 0, taxCategoryId);
            var category = await objSql.QueryFirstAsync<TaxCategoryDTO>(GET_TAX_CATEGORY_BY_ID);

            if (category == null)
            {
                return null;
            }

            objSql.NewCommand();
            objSql.AddParameter("@TaxCategoryId", DbType.Int32, ParameterDirection.Input, 0, taxCategoryId);
            var mappings = (await objSql.QueryAsync<TaxCategoryMappingDTO>(TAX_CATEGORY_MAPPING_SEL)).ToList();
            AttachMappings(category, mappings);
            return category;
        }

        public async Task<int> SaveTaxCategory(TaxCategoryDTO dto)
        {
            SQL objSql = new SQL();
            objSql.BeginTransaction();

            try
            {
                int taxCategoryId;

                objSql.NewCommand();
                objSql.AddParameter("@TaxName", DbType.String, ParameterDirection.Input, 0, dto.TaxName);

                if (dto.TaxCategoryId == 0)
                {
                    objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                    objSql.AddParameter("@CreatedBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
                    objSql.AddParameter("@CreatedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedOn);
                    taxCategoryId = Convert.ToInt32(await objSql.ExecuteScalarAsync(TAX_CATEGORY_INS));
                }
                else
                {
                    taxCategoryId = dto.TaxCategoryId;
                    objSql.AddParameter("@TaxCategoryId", DbType.Int32, ParameterDirection.Input, 0, dto.TaxCategoryId);
                    objSql.AddParameter("@ModifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);
                    objSql.AddParameter("@ModifiedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);
                    await objSql.ExecuteNonQueryAsync(TAX_CATEGORY_UPD);
                }

                await SyncTaxCategoryMappings(objSql, taxCategoryId, dto.TaxIds);
                objSql.Commit();
                return taxCategoryId;
            }
            catch
            {
                objSql.Rollback();
                throw;
            }
        }

        public async Task DeleteTaxCategory(int taxCategoryId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@TaxCategoryId", DbType.Int32, ParameterDirection.Input, 0, taxCategoryId);
            await objSql.ExecuteNonQueryAsync(TAX_CATEGORY_DEL);
        }

        private async Task SyncTaxCategoryMappings(SQL objSql, int taxCategoryId, List<int> taxIds)
        {
            objSql.NewCommand();
            objSql.AddParameter("@TaxCategoryId", DbType.Int32, ParameterDirection.Input, 0, taxCategoryId);
            await objSql.ExecuteNonQueryAsync(TAX_CATEGORY_MAPPING_DEL);

            if (taxIds == null || taxIds.Count == 0)
            {
                return;
            }

            foreach (var taxId in taxIds.Distinct())
            {
                if (taxId == 0)
                {
                    continue;
                }

                objSql.NewCommand();
                objSql.AddParameter("@TaxCategoryId", DbType.Int32, ParameterDirection.Input, 0, taxCategoryId);
                objSql.AddParameter("@TaxId", DbType.Int32, ParameterDirection.Input, 0, taxId);
                objSql.AddParameter("@IsDefault", DbType.Boolean, ParameterDirection.Input, 0, false);
                await objSql.ExecuteNonQueryAsync(TAX_CATEGORY_MAPPING_INS);
            }
        }

        public async Task<IEnumerable<TaxCategoryMappingDTO>> GetTaxesByCategoryId(int taxCategoryId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@TaxCategoryId", DbType.Int32, ParameterDirection.Input, 0, taxCategoryId);
            return await objSql.QueryAsync<TaxCategoryMappingDTO>(TAX_CATEGORY_MAPPING_SEL);
        }

        public async Task<IEnumerable<TaxMasterDTO>> GetAllTaxMasters(int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            return await objSql.QueryAsync<TaxMasterDTO>(TAX_MASTER_ALL);
        }

        public async Task<TaxMasterDTO> GetTaxMasterById(int id,int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@Id", DbType.Int32, ParameterDirection.Input, 0, id);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

            return await objSql.QueryFirstAsync<TaxMasterDTO>(TAX_MASTER_SEL);
        }

        public async Task<int> SaveTaxMaster(TaxMasterDTO dto)
        {
            SQL objSql = new SQL();
            AddTaxMasterFields(objSql, dto);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);

            if (dto.Id == 0)
            {
                // objSql.AddParameter("@Id", DbType.Guid, ParameterDirection.Input, 0, DBNull.Value);
                objSql.AddParameter("@CreatedBy", DbType.String, ParameterDirection.Input, 100, dto.CreatedBy);
                objSql.AddParameter("@CreatedDate", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedDate);
                var result = await objSql.ExecuteScalarAsync(TAX_MASTER_INS);
                return  Convert.ToInt32(result);
            }

            objSql.AddParameter("@Id", DbType.Int32, ParameterDirection.Input, 0, dto.Id);
            objSql.AddParameter("@ModifiedBy", DbType.String, ParameterDirection.Input, 100, dto.ModifiedBy);
            objSql.AddParameter("@ModifiedDate", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedDate);
            await objSql.ExecuteNonQueryAsync(TAX_MASTER_UPD);
            return dto.Id;
        }

        private void AddTaxMasterFields(SQL objSql, TaxMasterDTO dto)
        {
            objSql.AddParameter("@Name", DbType.String, ParameterDirection.Input, 100, dto.Name);
            objSql.AddParameter("@Code", DbType.String, ParameterDirection.Input, 20, dto.Code);
            objSql.AddParameter("@Country", DbType.String, ParameterDirection.Input, 10, (object)dto.Country ?? DBNull.Value);
            objSql.AddParameter("@Description", DbType.String, ParameterDirection.Input, 500, (object)dto.Description ?? DBNull.Value);
            objSql.AddParameter("@Rate", DbType.Decimal, ParameterDirection.Input, 0, dto.Rate);
            objSql.AddParameter("@RateType", DbType.String, ParameterDirection.Input, 20, dto.RateType);
            objSql.AddParameter("@IsActive", DbType.Boolean, ParameterDirection.Input, 0, dto.IsActive);
            objSql.AddParameter("@IsCompound", DbType.Boolean, ParameterDirection.Input, 0, dto.IsCompound);
            objSql.AddParameter("@IsDefault", DbType.Boolean, ParameterDirection.Input, 0, dto.IsDefault);
            objSql.AddParameter("@EffectiveFrom", DbType.Date, ParameterDirection.Input, 0, dto.EffectiveFrom);
            objSql.AddParameter("@EffectiveTo", DbType.Date, ParameterDirection.Input, 0, (object)dto.EffectiveTo ?? DBNull.Value);
            objSql.AddParameter("@ApplicableTo", DbType.String, ParameterDirection.Input, 50, (object)dto.ApplicableTo ?? DBNull.Value);
            objSql.AddParameter("@CustomerType", DbType.String, ParameterDirection.Input, 50, (object)dto.CustomerType ?? DBNull.Value);
            objSql.AddParameter("@Location", DbType.String, ParameterDirection.Input, 100, (object)dto.Location ?? DBNull.Value);
            objSql.AddParameter("@MinAmount", DbType.Decimal, ParameterDirection.Input, 0, (object)dto.MinAmount ?? DBNull.Value);
            objSql.AddParameter("@MaxAmount", DbType.Decimal, ParameterDirection.Input, 0, (object)dto.MaxAmount ?? DBNull.Value);
        }

        #region Procedures
        const string GET_ALL = "p_Taxes_getAll";
        const string ADD = "p_ApplcableTax_ins";
        const string UPDATE = "p_ApplcableTax_upd";
        const string APPLICABLE_TAXES = "p_getApplicableTaxes";
        const string GET_ALL_TAXCATOGRIES = "p_taxCategory_all";
        const string GET_TAX_CATEGORY_BY_ID = "p_taxCategory_sel";
        const string TAX_CATEGORY_INS = "p_taxCategory_ins";
        const string TAX_CATEGORY_UPD = "p_taxCategory_upd";
        const string TAX_CATEGORY_DEL = "p_taxCategory_del";
        const string TAX_CATEGORY_MAPPING_SEL = "p_taxCategoryMapping_selByCategory";
        const string TAX_CATEGORY_MAPPING_ALL = "p_taxCategoryMapping_all";
        const string TAX_CATEGORY_MAPPING_DEL = "p_taxCategoryMapping_delByCategory";
        const string TAX_CATEGORY_MAPPING_INS = "p_taxCategoryMapping_ins";
        const string TAX_MASTER_ALL = "p_TaxMaster_all";
        const string TAX_MASTER_SEL = "p_TaxMaster_sel";
        const string TAX_MASTER_INS = "p_TaxMaster_ins";
        const string TAX_MASTER_UPD = "p_TaxMaster_upd";

        //  const string APPLICABLE_TAXES = "p_getItemTax";

        #endregion
    }
}
