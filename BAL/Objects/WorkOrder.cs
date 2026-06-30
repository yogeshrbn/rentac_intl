using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.Enums;
using BAL.DTO;
using BAL.DAL;
using BAL.Interface;
using System.Data;
using System.ComponentModel.Design;
using BAL.Common;
namespace BAL.Objects
{
    public class WorkOrder : WorkOrderDTO
    {

        /// <summary>
        /// Initialize challan, pass id as 0 to create a new challan.
        /// </summary>
        /// <param name="id">Challan Id</param>
        public WorkOrder(int id)
            : base(id)
        {

            if (WorkOrderId > 0)
            {
                GetById();
            }
        }


        void GetById()
        {
            WorkorderDAL objDal = new WorkorderDAL();
            WorkOrderDTO dto = objDal.GetById(this.WorkOrderId);
            this.Client = dto.Client;
            this.Number = dto.Number;
            this.Site = dto.Site;
            this.ClientAmount = dto.ClientAmount;
            this.Company = dto.Company;
            this.CompanyId = dto.CompanyId;
            this.CreatedBy = dto.CreatedBy;
            this.SitePic = dto.SitePic;
            this.SubTotal = dto.SubTotal;
            this.Total = dto.Total;
            this.CompanyId = dto.CompanyId;
            this.LedgerId = dto.LedgerId;
            this.FinYearId = dto.FinYearId;

            this.RentStartDate = dto.RentStartDate;
            this.WorkOrderNumber = dto.WorkOrderNumber;
            this.WorkOrderDate = dto.WorkOrderDate;

            this.IGSTRate = dto.IGSTRate;
            this.SGSTRate = dto.SGSTRate;
            this.CGSTRate = dto.CGSTRate;
            this.IGSTAmount = dto.IGSTAmount;
            this.SGSTAmount = dto.SGSTAmount;
            this.CGSTAmount = dto.CGSTAmount;
            this.TotalTax = dto.TotalTax;
            this.Remarks = dto.Remarks;
            this.Tnc = dto.Tnc;
            this.SezDescription = dto.SezDescription;
            this.ShipFrom = dto.ShipFrom;
            this.Vehicle = dto.Vehicle;
            this.Driver = dto.Driver;

            this.RefNo = dto.RefNo;
            this.TransporterId = dto.TransporterId;
            this.TeamId = dto.TeamId;
            this.LedgerSiteId = dto.LedgerSiteId;
            this.RecoveryDate = dto.RecoveryDate;
            this.WarehouseId = dto.WarehouseId;
            this.Weight = dto.Weight;
            this.ApproximateValue = dto.ApproximateValue;
            this.LRNumber = dto.LRNumber;
            this.CRNumber = dto.CRNumber;
            this.GRNumber = dto.GRNumber;
            this.PONumber = dto.PONumber;
            this.PODate = dto.PODate;
            this.ProjectOwnerId = dto.ProjectOwnerId;
            this.ProjectOwnerName = dto.ProjectOwnerName;
            this.ProjectOwnerPhone  = dto.ProjectOwnerPhone;
        }

        public async Task<WorkOrderDTO> JobWoById(int workOrderId, int companyId)
        {
            WorkorderDAL objDal = new WorkorderDAL();
            return await objDal.JobWoById(workOrderId, companyId);
        }




        /// <summary>
        /// Saves the challan
        /// </summary>
        /// <returns>Newly Created/Modified Challan Id</returns>
        public int Save()
        {
            WorkorderDAL dal = new WorkorderDAL();
            int wOrderid = this.WorkOrderId;
            //if (this.WorkOrderId == 0)
            //{
            wOrderid = dal.Save(this);
            this.WorkOrderId = wOrderid;
            //}
            //else
            //{
            //    dal.UpdateWorkOrder(this);

            //}
            return wOrderid;
        }

        public List<SiteDTO> GetAll(string workOrderNumber, string jobNumber, string site, string client, bool closed, int companyId, string siteEng)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.GetAll(workOrderNumber, jobNumber, site, client, closed, companyId, siteEng);
        }
        public List<SiteDTO> GetByCompany(int companyId, int finYearId, string code)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.GetByCompany(companyId, finYearId, code);
        }
        public List<WorkOrderItemDTO> GetItems()
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.GetWorkOrderItems(this.WorkOrderId);
        }

        public List<SiteDTO> GetSites(int workOrderId)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.GetSites(workOrderId);
        }
        public bool AddSite(List<SiteDTO> sites, int workOrderId)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.CreateSite(sites, workOrderId);
        }
        public bool AddSiteItems(SiteDTO site, SiteItemType itemType = SiteItemType.DELIVERED)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.AddItems(site, itemType);
        }
        public List<WorkOrderItemDTO> GetSiteItems(int siteId)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.GetSiteItems(siteId);
        }
        /// <summary>
        /// Deletes the site
        /// </summary>
        /// <param name="workOrderId"></param>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public async Task<int> DeleteChallan(int workOrderId, int siteId, LoggedInUserInfo user)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return await dal.DeleteChallan(workOrderId, siteId, user);
        }
        public List<TaxDTO> GetSiteTaxes(int siteId)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.GetSiteTaxes(siteId);
        }

        public bool UpdateSiteInfo(SiteDTO siteInfo)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.UpdateSiteInfo(siteInfo);
        }
        public async Task<IEnumerable<WorkOrderItemDTO>> ItemIssued(string from, string to, Int16 challanType, int clientId, int ledgerSiteId, int companyId, string challanNo, string listStatusFilter = null)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return await dal.ItemIssued(from, to, challanType, clientId, ledgerSiteId, companyId, challanNo, listStatusFilter);
        }


        public async Task<IEnumerable<WorkOrderDTO>> WorkOrders(WorkOrderDTO dto)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return await dal.WorkOrders(dto);
        }
        public List<GRNDTO> ItemReceived(string from, string to, short challanType, int clientId, int ledgerSiteId, int companyId,
             string challanNo, string listStatusFilter = null)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.ItemReceived(from, to, challanType, clientId, ledgerSiteId, companyId, challanNo, listStatusFilter);
        }
        public DataSet ItemReceived_Report(int grnId)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.ItemReceived_Report(grnId);
        }
        public List<SiteDTO> GetClientSites(int ledgerId)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.GetClientSites(ledgerId);
        }
        public List<SiteDTO> GetWorkOrderBalance(int ledgerId, int companyId)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.GetWorkOrderBalance(ledgerId, companyId);
        }
        public List<SiteDTO> WorkOrderDueDateReminder(int ledgerId, int companyId)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.WorkOrderDueDateReminder(ledgerId, companyId);
        }
        public List<SiteDTO> WorkOrderOverDuesReminder(int ledgerId, int companyId)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.WorkOrderOverDuesReminder(ledgerId, companyId);
        }
        public DataSet ItemIssuedForPrint(int workOrderId, int companyId)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.ItemIssuedForPrint(workOrderId, companyId);
        }
        public DataSet GetChallanReportHeader(int workOrderId, int challanHeaderType = 0)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.GetChallanReportHeader(workOrderId, challanHeaderType);
        }
        public List<WorkOrderChageDTO> GetOtherChages(int workOrderId)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.GetOtherChages(workOrderId);
        }
        public bool AddOtherCharges(int workOrderId, List<WorkOrderChageDTO> charges)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.AddOtherCharges(workOrderId, charges);
        }
        public List<InvoiceChargeDTO> GetSiteOtherCharges(int ledgerSiteId, int companyId)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.GetSiteOtherCharges(ledgerSiteId, companyId);
        }

        public ChallanDocumentDTO AddChallanDocument(ChallanDocumentDTO dto)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.AddChallanDocument(dto);
        }
        public int DeleteChallanItem(int workOrderItemId)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.DeleteChallanItem(workOrderItemId);
        }
        public List<ChallanDocumentDTO> GetChallanDocuments(int workOrderId)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.GetChallanDocuments(workOrderId);
        }
        public Boolean DeleteChallanDocument(int challanDocumentId)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.DeleteChallanDocument(challanDocumentId);
        }
        public DataSet PendingChallanAcknoledgements(int finYearId, int companyId)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.PendingChallanAcknoledgements(finYearId, companyId);
        }
        public async Task<bool> TransferChallan(WorkOrder dto, GRN objGRN)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return await dal.TransferChallan(dto, objGRN);
        }
        public async Task<bool> UpdateTransferChallan(WorkOrder dto, GRN objGRN)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return await dal.UpdateTransferChallan(dto, objGRN);
        }
        public async Task<MaterialTransferDto> MatTransferById(int workOrderId, int companyId)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return await dal.MatTransferById(workOrderId, companyId);
        }
        public bool ItemAdjustment(WorkOrderDTO wo, GRNDTO grn)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.ItemAdjustment(wo, grn);
        }
        public DataSet MatAdjustList(string from, string to, int ledgerId, int ledgerSiteId, int companyId)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.MatAdjustList(from, to, ledgerId, ledgerSiteId, companyId);
        }
        public DataSet MatAdjustById(int workOrderId)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.MatAdjustById(workOrderId);
        }
        public bool UpdateEwayBIllNo(int workorderId, string ewayBillNo)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return dal.UpdateEwayBIllNo(workorderId, ewayBillNo);
        }
        public async Task<int> UpdateStatus(WorkOrderDTO dto)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return await dal.UpdateStatus(dto);
        }

        public async Task<IEnumerable<WorkOrderOperationDto>> GetOperations(int workorderId, int companyId)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return await dal.GetOperations(workorderId, companyId);
        }

        public async Task<bool> ChangeChallanParty(List<ChallanChangePartyDto> data, LoggedInUserInfo user)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return await dal.ChangeChallanParty(data, user);
        }

        public async Task<string> GetLastChallanNumber(WorkOrderDTO dto)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return await dal.GetLastChallanNumber(dto);
        }

        public async Task<string> GetNextChallanNumberPreview(WorkOrderDTO dto)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return await dal.GetNextChallanNumberPreview(dto);
        }
    }

    //public class WorkOrderTax : Tax
    //{
    //    int _siteId;
    //    public WorkOrderTax(int siteId)
    //    {
    //        _siteId = siteId;

    //    }

    //    public override void GetTaxes()
    //    {
    //        TaxDAL objTaxDAL = new TaxDAL();
    //        //return diffirent list in case of challanId>0, existing challans
    //        Taxes = objTaxDAL.GetTaxes(TaxItem.WorkOrder);

    //    }

    //    public override void SaveTax(int siteId = 0)
    //    {

    //    }


    //}

    
    public class MaterialTransferDto
    {
        public WorkOrderDTO WorkOrder { get; set; }
        public GRNDTO GRN { get; set; }

    }
}
