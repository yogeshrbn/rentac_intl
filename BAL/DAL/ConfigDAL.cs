using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BAL.DTO;
using BAL.Enums;
namespace BAL.DAL
{
  public  class ConfigDAL
    {
        public List<FinYearDTO> GetFinYearList()
        {
            SQL objSql = new SQL();
            return objSql.ContructList<FinYearDTO>(objSql.ExecuteDataSet(GET_ALL_FINYEAR));
        }
        public bool AddConfig(ConfigDTO dto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@Category", System.Data.DbType.String, System.Data.ParameterDirection.Input, 0, dto.Category);
            objSql.AddParameter("@SubCategory", System.Data.DbType.String, System.Data.ParameterDirection.Input, 0, dto.SubCategory);
            objSql.AddParameter("@Key", System.Data.DbType.String, System.Data.ParameterDirection.Input, 0, dto.Key);
            objSql.AddParameter("@Value", System.Data.DbType.String, System.Data.ParameterDirection.Input, 0, dto.Value);
            objSql.AddParameter("@CompanyId", System.Data.DbType.Int32, System.Data.ParameterDirection.Input, 0, dto.CompanyId);
            return objSql.ExecuteNonQuery(ADD_CONFIG) > 0;
        }
        public async Task<bool> AddConfigAsync(List<ConfigDTO> list)
        {
            SQL objSql = new SQL();
            try
            {
                objSql.BeginTransaction();
                int result = 0;
                foreach (var dto in list)
                {
                    objSql.NewCommand();
                    objSql.AddParameter("@Category", System.Data.DbType.String, System.Data.ParameterDirection.Input, 0, dto.Category);
                    objSql.AddParameter("@SubCategory", System.Data.DbType.String, System.Data.ParameterDirection.Input, 0, dto.SubCategory);
                    objSql.AddParameter("@Key", System.Data.DbType.String, System.Data.ParameterDirection.Input, 0, dto.Key);
                    objSql.AddParameter("@Value", System.Data.DbType.String, System.Data.ParameterDirection.Input, 0, dto.Value);
                    objSql.AddParameter("@CompanyId", System.Data.DbType.Int32, System.Data.ParameterDirection.Input, 0, dto.CompanyId);
                    result = await objSql.ExecuteNonQueryAsync(ADD_CONFIG);
                    if (result == 0)
                    {
                        throw new Exception($"Could not save configuration {dto.SubCategory}/{dto.Key}");
                    }
                }
                objSql.Commit();
                return true;
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                throw new Exception(ex.Message);
            }

        }

        public ConfigDTO GetValue(ConfigDTO dto, SQL sql = null)
        {
            SQL objSql;
            if (sql != null)
            {
                objSql = sql;
            }
            else
            {
                objSql = new SQL();
            }
            objSql.AddParameter("@Category", System.Data.DbType.String, System.Data.ParameterDirection.Input, 0, dto.Category);
            objSql.AddParameter("@SubCategory", System.Data.DbType.String, System.Data.ParameterDirection.Input, 0, dto.SubCategory);
            objSql.AddParameter("@Key", System.Data.DbType.String, System.Data.ParameterDirection.Input, 0, dto.Key);
            objSql.AddParameter("@CompanyId", System.Data.DbType.Int32, System.Data.ParameterDirection.Input, 0, dto.CompanyId);

            return objSql.ContructList<ConfigDTO>(objSql.ExecuteDataSet(GET_CONFIG_VALUE)).FirstOrDefault();
        }
        public List<ConfigDTO> GetBillingConfig(int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", System.Data.DbType.Int32, System.Data.ParameterDirection.Input, 0, companyId);
            var ds = objSql.ExecuteDataSet(GET_BILLING_CONFIG);

            var lst = (from d in ds.Tables[0].AsEnumerable()
                       select new ConfigDTO
                       {
                           Category = d.Field<string>("Category"),
                           SubCategory = d.Field<string>("SubCategory"),
                           Key = d.Field<string>("Key"),
                           Value = DBNull.Value == d["Value"] ? "" : d.Field<string>("Value"),

                       }).ToList();
            return lst;
        }

        public List<ConfigDTO> GetConfig(int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", System.Data.DbType.Int32, System.Data.ParameterDirection.Input, 0, companyId);
            return objSql.ContructList<ConfigDTO>(objSql.ExecuteDataSet(GET_CONFIG));
        }
        public List<ConfigDTO> GetConfig(int companyId, string category, string subCategory)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", System.Data.DbType.Int32, System.Data.ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@category", System.Data.DbType.String, System.Data.ParameterDirection.Input, 0, category);
            objSql.AddParameter("@subCategory", System.Data.DbType.String, System.Data.ParameterDirection.Input, 0, subCategory);

            return objSql.ContructList<ConfigDTO>(objSql.ExecuteDataSet(GET_CATEGORY_SUBCATEGORY_CONFIG));
        }
        public string GetKeyValue(ConfigCategory category, ConfigCategory subCategory, ConfigKey key, int companyId, SQL sql)
        {
            //  ConfigDAL configDal = new ConfigDAL();
            ConfigDTO cdto = new ConfigDTO();
            cdto.Category = Convert.ToString(category);
            cdto.SubCategory = Convert.ToString(subCategory);
            cdto.Key = Convert.ToString(key);
            cdto.CompanyId = companyId;
            cdto = this.GetValue(cdto, sql);
            if (cdto != null)
                return cdto.Value;
            else
                return null;
        }
        #region procedures
        const string GET_ALL_FINYEAR = "p_FinYear_sel";
        const string ADD_CONFIG = "p_config_ins";
        const string GET_CONFIG_VALUE = "p_config_getValue";
        const string GET_BILLING_CONFIG = "p_billingConfig_sel";
        const string GET_CONFIG = "p_Config_sel";
        const string GET_CATEGORY_SUBCATEGORY_CONFIG = "p_config_byCategoryAndSubCategory";
        #endregion
    }
}
