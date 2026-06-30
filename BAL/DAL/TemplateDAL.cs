using BAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace BAL.DAL
{
    public class TemplateDAL
    {

        public async Task<IEnumerable<Template>> GetByByGroup(string groupName)
        {
            var sql = new SQL();
            sql.AddDynamicParameter("@group", DbType.String, ParameterDirection.Input, 0, groupName);
            return await sql.QueryAsync<Template>(LIST_BY_GROUP);
        }


        #region procedures
        const string LIST_BY_GROUP = "p_templategallery_sel";
        #endregion
    }
}
