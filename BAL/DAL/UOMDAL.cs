using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using System.Data;
namespace BAL.DAL
{
    public class UOMDAL
    {
        public List<UOMDTO> GetAll(UOMDTO dto)
        {
            SQL objSql = new SQL();
            return objSql.ContructList<UOMDTO>(objSql.ExecuteDataSet(GET_ALL));
        }

        public List<UOMSizeDTO> GetSize(int uomId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@UOMId", DbType.Int16, ParameterDirection.Input, 0, uomId);
            return objSql.ContructList<UOMSizeDTO>(objSql.ExecuteDataSet(GET_UOM_SIZE));
        }

        #region Procedures
        const string GET_ALL = "p_UOM_getAll";
        const string GET_UOM_SIZE = "p_UOMSize_get";
        #endregion
    }
}
