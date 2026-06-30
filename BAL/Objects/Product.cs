using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DAL;
using System.Runtime.Remoting;
using NLog;
namespace BAL.Objects
{
    public class Product : ProductDTO
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public Product()
        {
        }
        public Product(int productid)
        {
            ProductId = productid;
            if (ProductId > 0)
            {
                GetInfo();
            }
        }

        async Task GetInfo()
        {
            ProductDAL objDal = new ProductDAL();
            try
            {
                ProductDTO objDTO = Task.Run<ProductDTO>(async () =>
                    await objDal.GetInfo(this.ProductId)
                ).Result;
                this.Name = objDTO.Name;
                this.Category = objDTO.Category;
                this.Code = objDTO.Code;
                this.CompanyId = objDTO.CompanyId;
                this.CST = objDTO.CST;
                this.CSTRate = objDTO.CSTRate;
                this.ExiseRate = objDTO.ExiseRate;
                this.LocalTax = objDTO.LocalTax;
                this.MRP = objDTO.MRP;
                this.Packing = objDTO.Packing;
                this.PackingQty = objDTO.PackingQty;
                this.ProductType = objDTO.ProductType;
                this.PurchaseRate = objDTO.PurchaseRate;
                this.Salt = objDTO.Salt;
                this.StoreId = objDTO.StoreId;
                this.Unit = objDTO.Unit;
                this.VatRate = objDTO.VatRate;
                this.UOM = objDTO.UOM;
                this.UOM2 = objDTO.UOM2;
                this.ApplyUnit2Rate = objDTO.ApplyUnit2Rate;
                this.Size = objDTO.Size;
                this.SortOrder = objDTO.SortOrder;
                this.Description = objDTO.Description;
                this.OpeningBalance = objDTO.OpeningStock;
                this.HSNCode = objDTO.HSNCode;
                this.Weight = objDTO.Weight;
                this.TaxCategoryId = objDTO.TaxCategoryId;
                this.SaleAccount = objDTO.SaleAccount;
                this.SalePrice = objDTO.SalePrice;
                this.CostPrice = objDTO.CostPrice;
                this.PurchaseAccount = objDTO.PurchaseAccount;
                this.PurchaseUnitId = objDTO.PurchaseUnitId;
                this.LossRate = objDTO.LossRate;
                this.RentRate = objDTO.RentRate;
                this.BrekageRate = objDTO.BrekageRate;
                this.SACCode = objDTO.SACCode;
                this.UnitSizeRate = objDTO.UnitSizeRate;
                this.WeightRate = objDTO.WeightRate;
                this.ItemGroupId = objDTO.ItemGroupId;
                this.Image1 = objDTO.Image1;

                this.TaxClassiFications = objDTO.TaxClassiFications;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        public async Task<List<ProductDTO>> GetAll(int companyId)
        {
            ProductDAL objDal = new ProductDAL();
            return await objDal.GetAll(companyId);
        }

        public async Task<bool> Save()
        {
            ProductDAL objDAL = new ProductDAL();
            bool ret = await objDAL.Save(this);

            //if success update opening balance
            if (ret)
            {
                //var stList = new List<StockTransactionDTO>();
                //var st = new StockTransactionDTO();
                //st.Remarks = "OP";
                //st.CompanyId = this.CompanyId;
                //st.CreatedOn = DateTime.Now;
                //st.FinYear = this.FinYearId;
                //st.PostedBy = this.CreatedBy;
                //st.PostingType = 1;
                //st.Quantity = this.OpeningBalance;
                //st.PostingDate = this.CreationDate;
                //st.VoucherId = "0";
                //stList.Add(st);
                //var stDal = new InventoryDAL();
                //stDal.PostStock(stList);
            }

            return ret;
        }
        public List<ProductDTO> Search(string query, int companyId)
        {
            ProductDAL objDal = new ProductDAL();
            return objDal.Search(query, companyId);
        }

        public void AddRate(RentRateDTO obj)
        {
            ProductDAL objDal = new ProductDAL();
            objDal.AddRate(obj);
        }
        public List<RentRateDTO> GetRates(int productId)
        {
            ProductDAL objDal = new ProductDAL();
            return objDal.GetRates(productId);
        }
        public Boolean ActivateRate(bool activate, int rentRateId)
        {
            ProductDAL objDal = new ProductDAL();
            return objDal.ActivateRate(activate, rentRateId);
        }

        public bool AddUOMSize(UOMSizeDTO dto)
        {
            ProductDAL objDal = new ProductDAL();
            return objDal.AddUOMSize(dto);

        }
        public List<UOMSizeDTO> GetALLSize()
        {
            ProductDAL objDal = new ProductDAL();
            return objDal.GetALLSize();

        }
        public async Task<bool> DeleteProductId(int productId, int companyId)

        {
            ProductDAL objDal = new ProductDAL();
            return await objDal.DeleteProductId(productId, companyId);
        }
        public bool ActviateSize(bool activate, Int32 uomSizeId)
        {
            ProductDAL objDal = new ProductDAL();
            return objDal.ActviateSize(activate, uomSizeId);

        }
        public bool AddProductSize(List<ProductSizeDTO> list, int createdBy, int finyear, int companyId)
        {
            ProductDAL objDal = new ProductDAL();
            return objDal.AddProductSize(list, createdBy, finyear, companyId);

        }
        public List<ProductSizeDTO> ProductSizeList(int productId, int finYearId)
        {
            ProductDAL objDal = new ProductDAL();
            return objDal.ProductSizeList(productId, finYearId);
        }
        public List<ProductSizeDTO> ProductSizeListByCompany(int companyId)
        {
            ProductDAL objDal = new ProductDAL();
            return objDal.ProductSizeListByCompany(companyId);
        }

        public async Task<bool> SaveBOM(BOMDTO dto)
        {
            ProductDAL objDal = new ProductDAL();
            return await objDal.SaveBom(dto);
        }
        public async Task<IEnumerable<BOMDTO>> BOMList(int companyId)
        {
            ProductDAL objDal = new ProductDAL();
            return await objDal.BOMList(companyId);
        }
        public async Task<BOMDTO> BOMById(int bomId, int companyId)
        {
            ProductDAL objDal = new ProductDAL();
            return await objDal.BOMById(bomId, companyId);
        }

        public async Task<BOMDTO> BOMByProductId(int productId, int companyId)
        {
            ProductDAL objDal = new ProductDAL();
            return await objDal.BOMByProductId(productId, companyId);
        }
        public async Task<bool> DeleteBOM(int bomId, int companyId)
        {
            ProductDAL objDal = new ProductDAL();
            return await objDal.DeleteBOM(bomId, companyId);
        }
        public async Task<int> SaveGroup(ItemGroupDTO dto)
        {
            ProductDAL objDal = new ProductDAL();
            return await objDal.SaveGroup(dto);
        }
        public async Task<IEnumerable<ItemGroupDTO>> ListItemGroup(ItemGroupDTO dto)
        {
            ProductDAL objDal = new ProductDAL();
            return await objDal.ListItemGroup(dto);
        }
        public async Task<ItemGroupDTO> ItemGroupById(ItemGroupDTO dto)
        {
            ProductDAL objDal = new ProductDAL();
            return await objDal.ItemGroupById(dto);
        }
        public async Task<int> DeleteItemGroup(ItemGroupDTO dto)
        {
            ProductDAL objDal = new ProductDAL();
            return await objDal.DeleteItemGroup(dto);
        }

    }
}
