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
    public class NextId
    {

        public string GenNextNumber(NextIDTables table, ConfigCategory category, ConfigCategory subCategory,
            int companyId, int finYear)
        {
            Config cnfig = new Config();
            string prefix = cnfig.GetKeyValue(category, subCategory, ConfigKey.Prefix, companyId, null);
            string start = cnfig.GetKeyValue(category, subCategory, ConfigKey.Start, companyId, null);

            string lastId = this.GetLastId(table, finYear.ToString(), companyId, prefix);
            if (String.IsNullOrEmpty(lastId))
            {
                // lastId  = getNextNumber(table, prefix, start);
                lastId = this.GetNextId(table, finYear.ToString(), companyId, start, prefix);
            }
            else
            {
                lastId = (Convert.ToInt16(lastId) + 1).ToString();
            }
            return lastId;
        }
        public string GenNextNumber(NextIDTables table, ConfigCategory category, ConfigCategory subCategory,
           int companyId, int finYear,string prefix, string start)
        {
            Config cnfig = new Config();
            //string prefix = cnfig.GetKeyValue(category, subCategory, ConfigKey.Prefix, companyId, null);
            //string start = cnfig.GetKeyValue(category, subCategory, ConfigKey.Start, companyId, null);

            string lastId = this.GetLastId(table, finYear.ToString(), companyId, prefix);
            if (String.IsNullOrEmpty(lastId))
            {
                // lastId  = getNextNumber(table, prefix, start);
                lastId = this.GetNextId(table, finYear.ToString(), companyId, start, prefix);
            }
            else
            {
                lastId = (Convert.ToInt16(lastId) + 1).ToString();
            }
            return lastId;
        }
        public string GetNextKeyWithPrefix(NextIDTables table, ConfigCategory category, ConfigCategory 
            subCategory, int companyId, int finYear)
        {
            Config cnfig = new Config();
            string prefix = cnfig.GetKeyValue(category, subCategory, ConfigKey.Prefix, companyId, null);
            return prefix + GenNextNumber(table, category, subCategory, companyId, finYear);
        }

        public string GetNextId(BAL.Enums.NextIDTables table, string finYear, int companyId, string start = "", string prefix = "")
        {
            NextIdDAL dal = new NextIdDAL();
            NextIdDTO dto = new NextIdDTO { Table = table, FinYear = finYear, CompanyId = companyId };
            dto.InitalValue = start;
            dto.Prefix = prefix;
            dto.InitalValue = String.IsNullOrEmpty(dto.InitalValue) ? "1000" : dto.InitalValue;
            return dal.GetNextId(dto);
        }
        public string GetLastId(BAL.Enums.NextIDTables table, string finYear, int companyId, string prefix)
        {
            NextIdDAL dal = new NextIdDAL();
            NextIdDTO dto = new NextIdDTO { Table = table, FinYear = finYear, CompanyId = companyId };
            dto.Prefix = prefix;
            return dal.GetLastId(dto);
        }

        public int UpdateId(BAL.Enums.NextIDTables table, string finYear, int companyId, string nextId, string prefix)
        {
            NextIdDAL dal = new NextIdDAL();
            NextIdDTO dto = new NextIdDTO { Table = table, FinYear = finYear, CompanyId = companyId };
            if (!String.IsNullOrEmpty(prefix))
            {
                if (!String.IsNullOrEmpty(nextId))
                {
                    nextId = nextId.Replace(prefix, "");
                }
            }
            else
            {
                prefix = "";
            }
            dto.NextId = nextId;
            dto.Prefix = prefix;
            return dal.UpdateId(dto);
        }
    }
}
