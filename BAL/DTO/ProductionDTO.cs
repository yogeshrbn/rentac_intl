using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{

    public class CreateProductionDto
    {
        public long ProductId { get; set; }
        public decimal Quantity { get; set; }
        public int StatusId { get; set; }
        public int CompanyId { get; set; }
        public long ClientId { get; set; }
        public string SaleOrderNo { get; set; }
        public string Description { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime ActualStartDate { get; set; }
        public DateTime ActualEndDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public long CreatedBy { get; set; }

        [Required(ErrorMessage = "At least one Production BOM item is required")]
        //[MinLength(1, ErrorMessage = "At least one Production BOM item is required")]       

        public List<CreateProductionBOMDto> BOM { get; set; }
        [Required(ErrorMessage = "At least one Operation is required")]
        public List<CreateProductionOperationDto> Operations { get; set; }
    }



    public class UpdateProductionDto
    {
        public long ProductId { get; set; }
        public decimal Quantity { get; set; }
        public int StatusId { get; set; }
        public int CompanyId { get; set; }
        public long ClientId { get; set; }
        public string SaleOrderNo { get; set; }
        public string Description { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime ActualStartDate { get; set; }
        public DateTime ActualEndDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public long ModifiedBy { get; set; }

        [Required(ErrorMessage = "At least one Production BOM item is required")]
        [MinLength(1, ErrorMessage = "At least one Production BOM item is required")]
        public List<UpdateProductionBOMDto> BOM { get; set; }
    }


    // DTOs/ProductionDto.cs

    public class ProductionDto
    {
        public long ProductionId { get; set; }
        public long ProductId { get; set; }
        public decimal Quantity { get; set; }
        public int StatusId { get; set; }
        public int CompanyId { get; set; }
        public long ClientId { get; set; }
        public string SaleOrderNo { get; set; }
        public string Description { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime ActualStartDate { get; set; }
        public DateTime ActualEndDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string GuId { get; set; } = string.Empty;

        public List<ProductionBOMDto> BOM { get; set; }
    }



    public class ProductionQueryDto
    {
        public int? CompanyId { get; set; }
        public long? ProductId { get; set; }
        public long? ClientId { get; set; }
        public int? StatusId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string SortColumn { get; set; } = "CreatedOn";
        public string SortDirection { get; set; } = "DESC";
    }


    // DTOs/PagedResultDto.cs

    public class PagedResultDto<T>
    {
        public List<T> Data { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);
    }

    public class ProductionBOMDto
    {
        public long ProductionBomId { get; set; }
        public long ProductId { get; set; }
        public long ProductionId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Consumed { get; set; }
        public decimal Returned { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string GuId { get; set; } = string.Empty;

        // Calculated properties
        public decimal RemainingQuantity => Quantity - Consumed + Returned;
        public decimal ConsumptionRate => Quantity > 0 ? (Consumed / Quantity) * 100 : 0;
    }

    public class ProductionOperationDto
    {
        public long ProductionOperationId { get; set; }
        public int OperationId { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int CompanyId { get; set; }

        public decimal Quantity { get; set; }
        public string GuId { get; set; }
        public short StatusId { get; set; }
    }

    public class CreateProductionOperationDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "OperationId must be greater than 0")]
        public int OperationId { get; set; } 
       
        [Required]
        [Range(0.1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal Quantity { get; set; }
      
    }
    public class UpdateProductionOperationDto : CreateProductionOperationDto
    {
        public long ProductionOperationId { get; set; }
    }
    public class ViewProductionOperationDto : ProductionOperationDto
    {
        public long ProductionOperationId { get; set; }
        public string StatusName { get; set; }
    }
    public class CreateProductionBOMDto
    {
        [Required]
        public long ProductId { get; set; }




        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Consumed { get; set; } = 0;

        [Range(0, double.MaxValue)]
        public decimal Returned { get; set; } = 0;


        public long CreatedBy { get; set; }

        public string GuId { get; set; }
    }
    public class UpdateProductionBOMDto
    {
        [Required]
        public long ProductionBomId { get; set; }

        public long? ProductId { get; set; }

        public long? ProductionId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal? Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Consumed { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Returned { get; set; }

        [Required]
        public long ModifiedBy { get; set; }
    }
}
