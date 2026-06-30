using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BAL.DTO;
using System.Data;
using BAL.Enums;
namespace BAL.DAL
{
    internal class ChallanDA
    {
        ChallanDTO _dataObject;
        internal long Save(ChallanDTO dataOBject)
        {
            SQL objSql = new SQL();
            long _challanId = 0;
            try
            {

                _dataObject = dataOBject;

                objSql.BeginTransaction();
                #region SaveChallan
                objSql.AddParameter("@ChallanNumber", DbType.String, ParameterDirection.Input, 50, dataOBject.ChallanNumber);
                objSql.AddParameter("@Type", DbType.SByte, ParameterDirection.Input, 50, dataOBject.ChallanType);
                objSql.AddParameter("@LedgerId ", DbType.Int32, ParameterDirection.Input, 50, dataOBject.Ledger.LedgerId);
                objSql.AddParameter("@ChallanDate", DbType.DateTime, ParameterDirection.Input, 50, dataOBject.ChallanDate);
                objSql.AddParameter("@SubTotal", DbType.Decimal, ParameterDirection.Input, 50, dataOBject.SubTotal);
                objSql.AddParameter("@TotalTax", DbType.Decimal, ParameterDirection.Input, 50, dataOBject.TotalTax);
                objSql.AddParameter("@Total", DbType.Decimal, ParameterDirection.Input, 50, dataOBject.Total);
                objSql.AddParameter("@DiscountRate", DbType.Double, ParameterDirection.Input, 50, dataOBject.DiscountRate);
                objSql.AddParameter("@Discount", DbType.Double, ParameterDirection.Input, 50, dataOBject.Discount);
                objSql.AddParameter("@Freight", DbType.Double, ParameterDirection.Input, 0, dataOBject.Freight);

                _challanId = Convert.ToInt64(objSql.ExecuteScalar(SAVE_CHALLAN));

                foreach (ChallanItemDTO item in dataOBject.Items)
                {
                    item.Challan = _dataObject;
                    objSql.NewCommand();
                    ChallanItemDAL itemDAL = new ChallanItemDAL();
                    itemDAL.Save(item, objSql);
                }
                foreach (TaxDTO item in dataOBject.ApplicableTaxes)
                {

                }
                objSql.Commit();
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                throw ex;
            }
                #endregion


            return _challanId;
        }

        internal List<WorkOrderDTO> GetChallanList(int ledgerId, int companyId, string from, string to, ChallanType type)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@Type", DbType.Int16, ParameterDirection.Input, 0, type);
            objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, from);
            objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, to);
            if (ledgerId > 0)
                objSql.AddParameter("@LedgerId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);

            return objSql.ContructList<WorkOrderDTO>(objSql.ExecuteDataSet(GET_CHALLAN_LIST));
        }
        
        internal int AddChallanToSite(int companyId, int siteWorkORderId, string challanNumber)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@WorkOrderId", DbType.Int32, ParameterDirection.Input, 0, siteWorkORderId);
            objSql.AddParameter("@challanNumber", DbType.String, ParameterDirection.Input, 0, challanNumber);


            return objSql.ExecuteNonQuery(ADD_CHALLAN_TO_SITE);
        }
        internal List<WorkOrderItemDTO> GetSiteChallans(int workOrderId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@workOrderId", DbType.Int32, ParameterDirection.Input, 0, workOrderId);


            return objSql.ContructList<WorkOrderItemDTO>(objSql.ExecuteDataSet(GET_SITE_CHALLANS));
        }
       
        #region procedures
        const string SAVE_CHALLAN = "p_Challan_ins";
        const string GET_CHALLAN_LIST = "p_challan_list";
        const string ADD_CHALLAN_TO_SITE = "p_SiteChallan_ins";
        const string GET_SITE_CHALLANS = "p_SiteChallan_sel";
    

        #endregion

    }
}
