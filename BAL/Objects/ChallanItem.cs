using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using BAL.DAL;
using BAL.DTO;
namespace BAL.Objects
{
    public class ChallanItem : WorkOrderItemDTO
    {
        Challan _challan;
        int _challanItemId;

        public int ChallanItemId { get { return _challanItemId; } }
        //Gets the Challan to which the item is belongs to
        public Challan Challan { get { return _challan; } }

        public ChallanItem()
        {
        }

        public ChallanItem(int id)
        {
            _challanItemId = id;
        }

         

    }
}
