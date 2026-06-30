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
    public class Sales
    {



        public bool CreateInvoice(SalesDTO salesDTO)
        {
            SalesDAL objSales = new SalesDAL();
            return objSales.Add(salesDTO);

        }



        public List<SalesDTO> SalesRegister(int companyId)
        {
            SalesDAL objSales = new SalesDAL();
            return objSales.SalesRegister(companyId);
        }
        public List<SalesItemDTO> SalesItemsList(int salesId)
        {
            SalesDAL objSales = new SalesDAL();
            return objSales.SalesItemsList(salesId);
        }
        public DataSet GetReceiptRegisterPRT(int salesId)
        {
            SalesDAL objSales = new SalesDAL();
            return objSales.GetReceiptRegisterPRT(salesId);
        }
        public List<SalesItemDTO> SalesItemsTax(int salesId)
        {
            SalesDAL objSales = new SalesDAL();

            return objSales.SalesItemsTax(salesId);
        }
    }
}
