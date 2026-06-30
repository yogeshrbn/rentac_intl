using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DAL;
using System.Data;
namespace BAL.Objects
{
    public class Purchase
    {



        public async Task<bool> CreateInvoice(PurchaseDTO purchaseDTO)
        {
            PurchaseDAL objBillingDAL = new PurchaseDAL();
            return await objBillingDAL.Add(purchaseDTO);

        }

        public List<BillingDTO> GetBilList(string from, string to, int companyId, int clientId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.GetBilList(from, to, companyId, clientId, 0, 0, 0);
        }
        public List<BillingItemDTO> BillItems(int billId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.BillItems(billId);
        }
        public DataSet PrintBill(int billId)
        {
            BillingDAL objBillingDAL = new BillingDAL();
            return objBillingDAL.PrintBill(billId);
        }


        public List<PurchaseDTO> PurchaseRegister(PurchaseFilterDTO dto)
        {
            PurchaseDAL objPurchase = new PurchaseDAL();
            return objPurchase.PurchaseRegister(dto);
        }
        public List<PurchaseItemDTO> PurchaseItemsList(int purchaseId)
        {
            PurchaseDAL objPurchase = new PurchaseDAL();
            return objPurchase.PurchaseItemsList(purchaseId);
        }
        public DataSet GetReceiptRegisterPRT(int purchaseId)
        {
            PurchaseDAL objPurchase = new PurchaseDAL();
            return objPurchase.GetReceiptRegisterPRT(purchaseId);
        }
        public List<PurchaseItemDTO> PurchaseItemsTax(int puchaseId)
        {
            PurchaseDAL objPurchase = new PurchaseDAL();
            return objPurchase.PurchaseItemsTax(puchaseId);
        }

        public async Task<PurchaseDTO> ById(int purchaseId, int companyId)
        {
            PurchaseDAL objPurchase = new PurchaseDAL();
            return await objPurchase.ById(purchaseId, companyId);
        }

        public async Task<IEnumerable<PurchaseDTO>> GetUnpaidBills(int ledgerId, int companyId)
        {
            PurchaseDAL objPurchase = new PurchaseDAL();
            return await objPurchase.GetUnpaidBills(ledgerId, companyId);
        }

        public async Task<IEnumerable<PurchaseDTO>> GetBillsByIds(string billIds, int companyId)
        {
            PurchaseDAL objPurchase = new PurchaseDAL();
            return await objPurchase.GetBillsByIds(billIds, companyId);
        }

        public async Task<bool> UpdateStatus(PurchaseDTO dto)
        {
            PurchaseDAL objPurchase = new PurchaseDAL();
            return await objPurchase.UpdateStatus(dto);
        }
    }
}
