using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.Enums;

namespace BAL.DAL
{
    /*
    internal class TaxDAL
    {

        public List<TaxDTO> GetTaxes(TaxItem item)
        {
            List<TaxDTO> lstTaxes = null;
            SQL objSql = new SQL();
            objSql.AddParameter("@ItemId", System.Data.DbType.Int16, System.Data.ParameterDirection.Input, 0, Convert.ToInt16(item));
            switch (item)
            {
                case TaxItem.WorkOrder:
                    lstTaxes = objSql.ContructList<TaxDTO>(objSql.ExecuteDataSet(APPLICABLE_TAXES));
                    break;

            }

            return lstTaxes;
        }
        public void Save(TaxDTO dto, SQL objSql)
        {
            objSql.AddParameter("@ItemId", System.Data.DbType.Int16, System.Data.ParameterDirection.Input, 0, dto.TaxAmount);

        }


        #region procedures
        const string APPLICABLE_TAXES = "p_getApplicableTaxes";
        #endregion

    } */
}
