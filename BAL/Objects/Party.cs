using BAL.DAL;
using BAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Objects
{
    public class Party
    {
        public List<GRNDTO> PartyReturns(string partyMobileNo, DateTime from, DateTime to)
        {
            PartyDAL grnDal = new PartyDAL();
            return grnDal.PartyReturns(partyMobileNo, from, to);
        }
        public List<WorkOrderItemDTO> PartyReceived(string partyMobileNo, DateTime from, DateTime to)
        {
            PartyDAL grnDal = new PartyDAL();
            return grnDal.PartyReceived(partyMobileNo, from, to);

        }
        public List<PartyStockBalanceDTO> PartyStockBalance(string partyPhone, int companyId)
        {
            PartyDAL grnDal = new PartyDAL();
            return grnDal.PartyStockBalance(partyPhone, companyId);
        }
    }
}
