using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.Enums;
using BAL.DAL;
using BAL.DTO;
using System.Data;
using Omu.ValueInjecter;
using BAL.DTO.View;
using System.ComponentModel.Design;
namespace BAL.Objects
{
    public class Inventory
    {
        InventoryDAL dal = new InventoryDAL();
        public List<StockTransactionDTO> ItemStock(int finYearId, int companyId)
        {
            return dal.ItemStock(finYearId, companyId);
        }

        public List<LedgerbalanceDTO> StockInhand(int companyId, string onDate, int warehouseId = 0)
        {
            return dal.StockInhand(companyId, onDate, warehouseId);
        }
        public List<StockInventoryDto> StockSummary(int companyId, string onDate, int warehouseId = 0)
        {

            var lst = dal.StockSummary(companyId, onDate, warehouseId);
            foreach (var item in lst)
            {
                item.BalanceInPurchaseUnit = item.PurchaseUnitQty;
                var size = item.Size == 0 ? 1 : item.Size;
                 
                if(item.PurchaseUnitId == 0 || item.PurchaseUnitId == 1 || item.PurchaseUnitId ==2  ||
                    item.PurchaseUnitId == 4 || item.PurchaseUnitId == 5)
                {
                    size = 1;
                }
                  
                var soldQtySize = Convert.ToDecimal(item.SaleQty) * size;

                var onFloorQtySize = Convert.ToDecimal(item.OnFloor) * size;
                onFloorQtySize = onFloorQtySize < 0 ? 0 : onFloorQtySize;

                var onSiteQtySize = Convert.ToDecimal(item.OnSite) * size;
                onSiteQtySize = onSiteQtySize < 0 ? 0 : onSiteQtySize;

                item.BalanceInPurchaseUnit -= (soldQtySize + onSiteQtySize);

            }
            return lst;
        }
        public bool PostStock(StockTransactionHeaderDTO dto)
        {

            return dal.PostStock(dto);
        }
        public List<StockTransactionHeaderViewDTO> StockAdjustmentList(int companyId, DateTime from, DateTime to)
        {
            var lst = dal.StockAdjustmentList(companyId, from, to);
            var lstDto = lst.Select(o => new StockTransactionHeaderViewDTO().InjectFrom(o)).Cast<StockTransactionHeaderViewDTO>().ToList();
            return lstDto;
        }

        public StockTransactionHeaderViewDTO StockAdjustmentDetails(int companyId, int transactionHeaderId)
        {
            var lst = dal.StockAdjustmentDetails(companyId, transactionHeaderId);
            var dto = new StockTransactionHeaderViewDTO();
            if (lst != null)
            {
                dto.InjectFrom(lst);

                dto.Items = lst.Items.Select(o => new StockTransactionViewDTO().InjectFrom(o)).Cast<StockTransactionViewDTO>().ToList();
            }
            return dto;
        }
        public int StockDelete(int companyid, int transactionHeaderId)
        {
            return dal.StockDelete(companyid, transactionHeaderId);
        }

        public async Task<int> UpdateItemBalance(int finyearId, int companyId, int productId)
        {
            return await dal.UpdateItemBalance(finyearId, companyId, productId);
        }
    }
}
