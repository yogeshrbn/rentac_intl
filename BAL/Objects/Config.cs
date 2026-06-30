using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DAL;
using BAL.Enums;
namespace BAL.Objects
{
    public class Config
    {
        public static bool IsDispatchInwardNotificationEnabled(int companyId)
        {
            ConfigDAL dal = new ConfigDAL();
            var list = dal.GetConfig(companyId, "general", "notifications");
            var key = list?.FirstOrDefault(o => o.Key.Equals("notifyPartyOnDispatchInwardConfirm", StringComparison.OrdinalIgnoreCase));
            return key != null && key.Value != null && key.Value.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        public static List<FinYearDTO> GetFinYearList()
        {
            ConfigDAL dal = new ConfigDAL();
            return dal.GetFinYearList();
        }

        public bool AddConfig(ConfigDTO dto)
        {
            ConfigDAL dal = new ConfigDAL();
            return dal.AddConfig(dto);
        }
        public async Task<bool> AddConfigAsync(List<ConfigDTO> list)
        {
            ConfigDAL dal = new ConfigDAL();
            return await dal.AddConfigAsync(list);
        }
        public ConfigDTO GetValue(ConfigDTO dto)
        {
            ConfigDAL dal = new ConfigDAL();
            return dal.GetValue(dto);
        }
        public List<ConfigDTO> GetBillingConfig(int companyId)
        {
            ConfigDAL dal = new ConfigDAL();
            return dal.GetBillingConfig(companyId);
        }
        public List<ConfigDTO> GetAll(int companyId)
        {
            ConfigDAL dal = new ConfigDAL();
            return dal.GetConfig(companyId);
        }
        public List<ConfigDTO> GetConfig(int companyId, string category, string subCategory)
        {
            ConfigDAL dal = new ConfigDAL();
            return dal.GetConfig(companyId, category, subCategory);
        }
        public string GetKeyValue(ConfigCategory category, ConfigCategory subCategory, ConfigKey key, int companyId, SQL sql)
        {
            ConfigDAL dal = new ConfigDAL();
            return dal.GetKeyValue(category, subCategory, key, companyId, sql);
        }
    }
}
