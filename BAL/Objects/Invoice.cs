using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DAL;
using BAL.DTO;
namespace BAL.Objects
{
    public class Invoice
    {
        public bool Add(WorkOrderDTO dto)
        {
            InvoiceDAL invDAL = new InvoiceDAL();
            return invDAL.Add(dto);
        }

        public List<WorkOrderDTO> InvoiceList(int companyId)
        {
            InvoiceDAL invDAL = new InvoiceDAL();
            return invDAL.InvoiceList(companyId);
        }
    }
}
