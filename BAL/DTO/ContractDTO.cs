using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BAL.DTO
{

    public class ContractDTO
    {
        public int InvoiceId { get; set; }
        public int ContractId { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public int LedgerId { get; set; }
        public int LedgerSiteId { get; set; }
        public int TaxCategoryId { get; set; }

        public byte ContractType { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime NextBillDate { get; set; }
        public bool IsBillDue { get; set; }

        public DateTime ValidTill { get; set; }

        public short Duration { get; set; }
        public float Rate { get; set; }
        public short MeasureType { get; set; }
        public float ContractValue { get; set; }
        public float Area { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string GuId { get; set; }
        public byte StatusId { get; set; }
        public byte CalculationType { get; set; }
        public float BilledAmount { get; set; }
        public DateTime LastBilledDate { get; set; }
        public float LastBillAmount { get; set; }
        public string Remarks { get; set; }
        public int ProjectOwner { get; set; }
        public string ProjectOwnerName { get; set; }

        public string BillCycle { get; set; }
        public string ClientRefNo { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
        public float ApproximateWeight { get; set; }
        public string PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public short BillDay { get; set; }
        public string Doc1 { get; set; }
        public string Doc2 { get; set; }
        public string Doc3 { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public virtual List<ContractDetailDTO> Details { get; set; }
        public virtual List<ContractConditionDTO> Conditions { get; set; }

        public List<JobCardDto> JobCards { get; set; }
        public string SizeDescription { get; set; }
        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
        public string LocalityName { get; set; }
        public int LocalityId { get; set; }
        public int QuotationId { get; set; }
        public string QuotationNumber { get; set; }

    }

    public class ContractDetailDTO
    {
        public int ContractId { get; set; }
        public int ContractDetailId { get; set; }
        public int CompanyId { get; set; }
        public int ProductId { get; set; }
        public float Quantity { get; set; }
        public DateTime CreatedOn { get; set; }
        public string GuId { get; set; }
        public int CreatedBy { get; set; }
        public Byte Deleted { get; set; }
    }

    public class ContractConditionDTO
    {
        public int ContractConditionId { get; set; }
        public int ContractId { get; set; }
        public int CompanyId { get; set; }
        public string Condition { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string GuId { get; set; }
        public Byte Deleted { get; set; }

    }


    public class ContractViewDto : ContractDTO
    {
        public string Client { get; set; }
        public CompanyDTO Company { get; set; }
        public LedgerDTO Ledger { get; set; }
        public string SiteAddress { get; set; }
        public string StatusName { get; set; }
     
        public DateTime InstalledDate { get; set; }
        public DateTime DismantaledDate { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime ReInstalledDate { get; set; }

        public DateTime InstallDueDate { get; set; }
        public DateTime DismantleDueDate { get; set; }
        public DateTime PickupDueDate { get; set; }
        public DateTime ReInstalDueDate { get; set; }


        [XmlElement(typeof(ContractDetailViewDto))]
        public new List<ContractDetailViewDto> Details { get; set; }

        [XmlElement(typeof(ContractConditionViewDto))]
        public new List<ContractConditionViewDto> Conditions { get; set; }
        public float ActivityArea { get; set; }
        public string Activity { get; set; }
        public string ActivityStatus { get; set; }

        public DateTime ActivityDate { get; set; }
        
        public DateTime ActivityCompletionDate { get; set; }

        public int JobCardId { get; set; }

        public decimal ActivtyAreaCovered { get; set; }

        public string InstallStatus { get; set; }
        public string DismantleStatus { get; set; }
        public string PickupStatus { get; set; }
        
        public QuotationDTO Quotation { get; set; }

        /// <summary>All quotations linked to this contract (<see cref="QuotationDTO.ContractId"/> or legacy <see cref="ContractDTO.QuotationId"/>).</summary>
        public List<QuotationDTO> ContractQuotations { get; set; }

        public short DeliveryChallans { get; set; }

    }

    public class ContractDetailViewDto : ContractDetailDTO
    {
        public string Product { get; set; }

    }

    public class ContractConditionViewDto : ContractConditionDTO
    {

    }

    public class ContractFilterDto
    {
        public int CompanyId { get; set; }
        public long LedgerId { get; set; }
        public int ContractId { get; set; }

        public long LedgerSiteId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int CreatedBy { get; set; }
        public byte StatusId { get; set; }

        public string FilterFor { get; set; }

        public byte ActivityType { get; set; }
        public byte ActivityStatusId { get; set; }

        public bool Extended { get; set; }
        /// <summary>
        /// 0-ALL
        /// 1-Today
        /// 2-This Week
        /// 3-Next Week
        /// 4-Next Month
        /// </summary>
        public byte ExpringOn { get; set; }
        /// <summary>
        /// 0-ALL
        /// 1-Today
        /// 2-This Week
        /// 3-Next Week
        /// 4-Next Month
        /// </summary>
        public byte DueBillOn { get; set; }

        public string QuotationNumber { get; set; }

        /// <summary>Optional range filters (null = no bound). Applied after GetAll.</summary>
        public double? AreaMin { get; set; }
        public double? AreaMax { get; set; }
        public double? ContractValueMin { get; set; }
        public double? ContractValueMax { get; set; }
        public double? RateMin { get; set; }
        public double? RateMax { get; set; }
    }

    public class ContractUpdateStatusDto
    {
        public int ContractId { get; set; }
        public int CompanyId { get; set; }
        public byte StatusId { get; set; }
        public string Remarks { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }

    }


    public class ContractBillingDto : BaseDTO
    {
        public int ContractId { get; set; }
        public int InvoiceId { get; set; }
        public float IGST { get; set; }
        public float CGST { get; set; }
        public float SGST { get; set; }

        public float IGSTRate { get; set; }
        public float CGSTRate { get; set; }
        public float SGSTRate { get; set; }

        public string Remarks { get; set; }

        public float SubTotal { get; set; }
        public float Total { get; set; }

    }

    // Models/ContractActivity.cs
    public class ContractActivity : BaseDTO
    {
        public int ActivityId { get; set; }

        public int ContractId { get; set; }
        public byte ContractStatus { get; set; }
        public int TeamId { get; set; }
        public string Employees { get; set; }
        public string ActivityStatus { get; set; }
        public byte ActivityType { get; set; }

    }

    // Models/ContractActivitySearchFilter.cs
    public class ContractActivitySearchFilter
    {
        public int? CompanyId { get; set; }
        public int? ContractId { get; set; }
        public byte? ContractStatus { get; set; }
        public int? TeamId { get; set; }
        public string ActivityStatus { get; set; }
        public byte? ActivityType { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string SortColumn { get; set; } = "ActivityId";
        public string SortDirection { get; set; } = "DESC";
    }

    // Models/PagedResult.cs
    public class PagedResult<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    public class ContractActivityTrackerDTO
    {
        public int JobCardId { get; set; }
        public string ClientRefNo { get; set; }
        public string QuotationNumber { get; set; }
        public string Client { get; set; }
        public string Address { get; set; }
        public string AreaZone { get; set; }
        public string Locality { get; set; }
        public string SizeDescription { get; set; }
        public string Area { get; set; }
        public string TypeOfWork { get; set; }
        public string EstimatedEndDate { get; set; }
        public string EstimatedStartDate { get; set; }
        public string Employees { get; set; }
        public string Status { get; set; }
      

    }
    public class EmployeeDPRDTO
    {
        public int JobCardId { get; set; }
        public string ClientRefNo { get; set; }
        public string QuotationNumber { get; set; }
        public string Client { get; set; }
        public string Address { get; set; }
        public string AreaZone { get; set; }
        public string Locality { get; set; }
        public string SizeDescription { get; set; }
        public string Area { get; set; }
        public string TypeOfWork { get; set; }
        public string EstimatedEndDate { get; set; }
        public string EstimatedStartDate { get; set; }
        public string Employee { get; set; }
        public double AreaCovered { get; set; }
        public int TotalEmployees { get; set; }

        public string Status { get; set; }


    }

    /// <summary>Metadata for files linked to a contract (e.g. install activity images).</summary>
    public class ContractDocumentDto
    {
        public int ContractDocumentId { get; set; }
        public int CompanyId { get; set; }
        public int ContractId { get; set; }
        public int? JobCardId { get; set; }
        public string DocumentType { get; set; }
        public string StoragePath { get; set; }
        public string OriginalFileName { get; set; }
        public string ContentType { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
