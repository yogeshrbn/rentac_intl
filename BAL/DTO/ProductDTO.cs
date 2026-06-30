using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class ProductDTO : MasterDTO
    {
        public String Name { get; set; }
        public String Code { get; set; }
        public String Product { get; set; }
        public String BOMDescription { get; set; }
        public String BOMCode { get; set; }

        public string HSNCode { get; set; }

        public String Unit { get; set; }
        public String Packing { get; set; }

        public int CompanyId { get; set; }
        public int Salt { get; set; }
        public int ProductType { get; set; }

        public double LocalTax { get; set; }
        public double VatRate { get; set; }
        public double CSTRate { get; set; }
        public double ExiseRate { get; set; }
        public double PurchaseRate { get; set; }
        public double CostPrice { get; set; }

        public double SalePrice { get; set; }
        public double RentRate { get; set; }
        public double BrekageRate { get; set; }

        public double Rate { get; set; }
        public double MRP { get; set; }
        public double LossRate { get; set; }
        
        public double PackingQty { get; set; }

        public int Category { get; set; }
        public int ProductId { get; set; }
        public string SACCode { get; set; }

        public bool CST { get; set; }
        public int SaleAccount { get; set; }
        public int PurchaseAccount { get; set; }
        public Int16? PurchaseUnitId { get; set; }
        public short TaxCategoryId { get; set; }

        public int StoreId { get; set; }
        public int Status { get; set; }
        public double UnitSizeRate { get; set; }
        public double Weight { get; set; }
        public double WeightRate { get; set; }
        public int ItemGroupId { get; set; }
        public Int16 UOM { get; set; }
        /// <summary>Optional secondary UOM (same master list as UOM).</summary>
        public Int16? UOM2 { get; set; }
        /// <summary>When set, rent billing may use <see cref="UnitSizeRate"/> (with company billing rules).</summary>
        public bool ApplyUnit2Rate { get; set; }
        public string Unit2Name { get; set; }
        public string PurchaseUnitName { get; set; }

        public String Size { get; set; }
        public String Description { get; set; }
        public Int16 SortOrder { get; set; }
        //  List<ProductSizeDTO> _sizeList;
        public List<ProductSizeDTO> ProductSize { get; set; }
        public List<ProductTaxClassificationDto> TaxClassiFications { get; set; }


        //this property is only for assing from db to dto. Do not use 
        [Obsolete("only for assining from db to dto. User OpeningBalance for other usage")]
        public double OpeningStock { get; set; }

        public double OpeningBalance { get; set; }

        public string Image1 { get; set; }
        public ProductDTO()
        {
            ProductSize = new List<ProductSizeDTO>();
        }
    }

    public class ProductSizeDTO : ProductDTO
    {
        public int ProductSizeId { get; set; }
        public string Code { get; set; }
    }

    public class BOMDTO
    {
        public int BomId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public int ProductId { get; set; }
        public int CompanyId { get; set; }
        public short Quantity { get; set; }
        public string Product { get; set; }
        public string GuId { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }
        public List<BomDetails> Details { get; set; }

    }

    public class BomDetails
    {
        public int BomId { get; set; }
        public int BomDetailId { get; set; }
        public int CompanyId { get; set; }
        public int ProductId { get; set; }
        public string Product { get; set; }
        public string Unit { get; set; }

        public double Quantity { get; set; }
        public double Rate { get; set; }
        public double Amount { get; set; }

        public string GuId { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }

        public int UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }

    }

    public class ProductTaxClassificationDto
    {
        public long TaxClassificationId { get; set; }
        public int CompanyId { get; set; }
        public int ProductId { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public int TaxCategoryId { get; set; }
        public string Nature { get; set; }
        public string HsnCode { get; set; }
        public string SacCode { get; set; }
        public bool IsReverseCharge { get; set; }
        public bool IsExempt { get; set; }
        public bool IsNillRated { get; set; }
        public bool IsZeroRated { get; set; }


    }

    public class SaveProductTaxClassificationDto : ProductTaxClassificationDto
    {
     
    }
    public class ProductTaxClassificationFilterDto
    {
        public int CompanyId { get; set; }
        public int ProductId { get; set; }
        public string TransactionType { get; set; } = string.Empty;
    }
}
