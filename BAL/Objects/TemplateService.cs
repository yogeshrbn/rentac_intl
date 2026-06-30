using BAL.DAL;
using BAL.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Objects
{
    public class TemplateService
    {
        public async Task<IEnumerable<Template>> GetByByGroup(string groupName)
        {
            var dal = new TemplateDAL();
            return await dal.GetByByGroup(groupName);
        }
        public async Task<Template> GetDefaultPdfTemplate(int companyId, string groupName, string category, string subCategory)
        {
            var config = new Config();
            var configs = config.GetAll(companyId);
            var templates = await GetByByGroup(groupName);
            if (configs != null)
            {
                var configValue = configs.Where(o => o.Category.ToLower() == category.ToLower() && o.SubCategory == subCategory.ToLower() && o.Key == "pdftemplate").FirstOrDefault();
                var templateCss = configs.Where(o => o.Category.ToLower() == category.ToLower() && o.SubCategory == subCategory.ToLower() && o.Key == "css").FirstOrDefault();

                if (configValue != null)
                {
                    var template = templates.Where(o => o.TemplateId == Convert.ToInt16(configValue.Value)).FirstOrDefault();
                    if (templateCss != null && template != null)
                    {
                        template.Style = templateCss.Value;
                    }
                    return template;
                }
            }
            return templates.Where(o => o.IsDefault == true).FirstOrDefault();

        }
    }
}
