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

        #region Procedures
        const string GET_ALL = "p_Taxes_getAll";
        const string ADD = "p_ApplcableTax_ins";
        const string UPDATE = "p_ApplcableTax_upd";
        const string APPLICABLE_TAXES = "p_getApplicableTaxes";


        //  const string APPLICABLE_TAXES = "p_getItemTax";

        #endregion
    }
}
