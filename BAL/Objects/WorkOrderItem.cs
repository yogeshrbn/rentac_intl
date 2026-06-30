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
    public class WorkOrderItem : WorkOrderItemDTO
    {
        WorkOrder _workOrder;
        int _workOrderItemId;

        public int ChallanItemId { get { return _workOrderItemId; } }
        //Gets the Challan to which the item is belongs to
        public WorkOrder Challan { get { return _workOrder; } }

        public WorkOrderItem()
        {
        }

        public WorkOrderItem(int id)
        {
            _workOrderItemId = id;
        }

         

    }
}
