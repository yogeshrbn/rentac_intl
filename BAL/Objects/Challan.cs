using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.Enums;
using BAL.DTO;
using BAL.DAL;
using BAL.Interface;
using System.Data;
namespace BAL.Objects
{
    public class Challan : ChallanDTO
    {



        /// <summary>
        /// Initialize challan, pass id as 0 to create a new challan.
        /// </summary>
        /// <param name="id">Challan Id</param>
        public Challan(int id)
            : base(id)
        {

            if (ChallanId > 0)
            {
                //get challan Items
                //get Ledger
              //  Tax objTax = new ChallanTax(1);

            }
        }







        /// <summary>
        /// Saves the challan
        /// </summary>
        /// <returns>Newly Created/Modified Challan Id</returns>
        public int Save()
        {
            ChallanDA dal = new ChallanDA();
            long challanId = dal.Save(this);

            return 0;
        }

        public List<WorkOrderDTO> GetChallanList(int ledgerId,int companyId, string from, string to, ChallanType type)
        {
            ChallanDA dal = new ChallanDA();
            return dal.GetChallanList(ledgerId,companyId,from, to, type);
        }

        public int AddChallanToSite(int companyId, int siteWorkORderId, string challanNumber)
        {
            ChallanDA dal = new ChallanDA();
            return dal.AddChallanToSite(companyId, siteWorkORderId, challanNumber);
        }
        public List<WorkOrderItemDTO> GetSiteChallans(int workOrderId)
        {
            ChallanDA dal = new ChallanDA();
            return dal.GetSiteChallans(workOrderId);
        }
       
    }

    public class ChallanTax
    {
        int _challanId;
        public ChallanTax(int challanId)
        {
            _challanId = challanId;

        }

        //public override void GetTaxes()
        //{
        //    TaxDAL objTaxDAL = new TaxDAL();
        //    //return diffirent list in case of challanId>0, existing challans
        //    Taxes = objTaxDAL.GetTaxes(TaxItem.WorkOrder);

        //}

        //public override void SaveTax(int challanId = 0)
        //{

        //}


    }
}
