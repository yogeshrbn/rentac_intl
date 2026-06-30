using BAL.DAL;
using BAL.DTO;
using BAL.Objects;
using BAL.Services;
using FarmaAPI.Helper;
using Newtonsoft.Json.Linq;
using Omu.ValueInjecter;
using PdfSharp.Pdf.Content.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Razor.Generator;
using System.Web.Services;
using System.Windows.Controls;
using System.Windows.Interop;
namespace FarmaAPI.Controllers
{
    [Authorize]
    public class ContractController : BaseApiController
    {
        private static readonly string docsPath = Path.Combine(HttpRuntime.AppDomainAppPath, "docs");

        /// <summary>Null min/max = no bound. If both set and min &gt; max, swap for UX.</summary>
        private static bool WithinOptionalRange(double value, double? min, double? max)
        {
            double? lo = min;
            double? hi = max;
            if (lo.HasValue && hi.HasValue && lo.Value > hi.Value)
            {
                var t = lo;
                lo = hi;
                hi = t;
            }
            if (lo.HasValue && value < lo.Value)
                return false;
            if (hi.HasValue && value > hi.Value)
                return false;
            return true;
        }

        private static bool PassesContractRangeFilters(ContractViewDto d, ContractFilterDto f)
        {
            if (!WithinOptionalRange(d.Area, f.AreaMin, f.AreaMax))
                return false;
            if (!WithinOptionalRange(d.ContractValue, f.ContractValueMin, f.ContractValueMax))
                return false;
            if (!WithinOptionalRange(d.Rate, f.RateMin, f.RateMax))
                return false;
            return true;
        }

        [HttpPost]
        public async Task<ApiMessage> Save([FromBody] ContractDTO dto)
        {
            ApiMessage response = new ApiMessage();
            try
            {
                var contract = new Contract();
                var user = new LoggedInUser();
                dto.CreatedBy = user.UserId;
                dto.CreatedOn = DateTime.Now;
                dto.CompanyId = user.DefaultCompanyId;

                dto.GuId = Guid.NewGuid().ToString();
                if (dto.ContractType == 2)
                {
                    dto.ContractValue = dto.Rate * dto.Area;
                }

                if (dto.ContractId > 0 && dto.StatusId != 1)
                {
                    throw new Exception("Only draft contracts can be edited");
                }
                if (string.IsNullOrEmpty(dto.Remarks))
                {
                    throw new Exception("Please enter contract details");

                }
                var azService = new AzureStorageService();
                if (!String.IsNullOrEmpty(dto.Doc1))
                {
                    //  var doc1 = Convert.FromBase64String(dto.Doc1);

                    var file = new DataUrlHelper();

                    var isValidFile = file.IsDataUrl(dto.Doc1);
                    if (isValidFile)
                    {
                        dto.Doc1 = Guid.NewGuid().ToString();
                        dto.Doc1 = user.FinYearId + "/" + user.DefaultCompanyId + "/contracts" + "/" + dto.Doc1 + "." + file.Extension;
                        byte[] doc1 = new byte[file.FileStream.Length];
                        file.FileStream.Read(doc1, 0, Convert.ToInt32(file.FileStream.Length) - 1);
                        if (doc1 != null)
                        {
                            await azService.UploadFileAsync(
                                dto.Doc1, doc1);
                        }
                    }
                    else if (dto.Doc1.ToLower() != "delete")
                    {
                        dto.Doc1 = "";
                    }
                }
                if (!String.IsNullOrEmpty(dto.Doc2))
                {
                    //  var doc1 = Convert.FromBase64String(dto.Doc1);

                    var file = new DataUrlHelper();
                    var isValidFile = file.IsDataUrl(dto.Doc2);
                    if (isValidFile)
                    {
                        file.IsDataUrl(dto.Doc2);
                        dto.Doc2 = Guid.NewGuid().ToString();
                        dto.Doc2 = user.FinYearId + "/" + user.DefaultCompanyId + "/contracts" + "/" + dto.Doc2 + "." + file.Extension;
                        byte[] doc1 = new byte[file.FileStream.Length];
                        file.FileStream.Read(doc1, 0, Convert.ToInt32(file.FileStream.Length) - 1);
                        if (doc1 != null)
                        {
                            await azService.UploadFileAsync(
                                dto.Doc2, doc1);
                        }
                    }
                    else if (dto.Doc2.ToLower() != "delete")
                    {
                        dto.Doc2 = "";
                    }
                }

                if (!String.IsNullOrEmpty(dto.Doc3))
                {
                    //  var doc1 = Convert.FromBase64String(dto.Doc1);

                    var file = new DataUrlHelper();
                    var isValidFile = file.IsDataUrl(dto.Doc3);
                    if (isValidFile)
                    {
                        file.Parse(dto.Doc3);
                        dto.Doc3 = Guid.NewGuid().ToString();
                        dto.Doc3 = user.FinYearId + "/" + user.DefaultCompanyId + "/contracts" + "/" + dto.Doc3 + "." + file.Extension;
                        byte[] doc1 = new byte[file.FileStream.Length];
                        file.FileStream.Read(doc1, 0, Convert.ToInt32(file.FileStream.Length) - 1);
                        if (doc1 != null)
                        {
                            await azService.UploadFileAsync(
                               dto.Doc3, doc1);
                        }
                    }
                    else if (dto.Doc3.ToLower() != "delete")
                    {

                        dto.Doc3 = "";
                    }
                }

                response.Data = await contract.Save(dto);
                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;

            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;

            }
            return response;
        }

        [HttpPost]
        public async Task<IHttpActionResult> Filter([FromBody] ContractFilterDto dto)
        {
            ApiMessage response = new ApiMessage();
            try
            {
                var contract = new Contract();
                var user = new LoggedInUser();

                dto.CompanyId = user.DefaultCompanyId;
                if (String.IsNullOrEmpty(dto.FilterFor))
                {
                    dto.FilterFor = "all";
                }
                var data = await contract.GetAll(dto);
                var filteredData = new List<ContractViewDto>();

                foreach (var d in data)
                {


                    var dateToCheckLastBill = d.LastBilledDate;
                    DateTime billDueDate;


                    if (dateToCheckLastBill.Year < 2000)
                    {
                        dateToCheckLastBill = d.EffectiveFrom;
                    }




                    billDueDate = d.EffectiveFrom.AddMonths(1);

                    var nextMonth = DateTime.Today.AddMonths(1);
                    var nextBillDate = nextMonth.AddDays(-(nextMonth.Day - 1));


                    d.NextBillDate = nextBillDate;

                    //if (Utils.GetDaysDifference(DateTime.Today,d.NextBillDate) <= 7)
                    //{
                    //    d.IsBillDue = true;
                    //}
                    //if no bill genereted yet

                    //if bill due in next seven days 
                    if (Utils.GetDaysDifference(DateTime.Today, d.NextBillDate) <= 7)
                    {
                        //check if bill already genereted
                        if (d.LastBilledDate.Year > 2000)
                        {
                            if (Utils.GetMonthDifference(dateToCheckLastBill, DateTime.Today) > 0)
                            {
                                d.IsBillDue = true;
                            }
                        }
                        else
                        {
                            d.IsBillDue = true;
                        }
                    }

                    if (Utils.GetDaysDifference(dateToCheckLastBill, DateTime.Today) > 7)
                    {
                        //check if bill already genereted
                        if (d.LastBilledDate.Year > 2000)
                        {
                            if (Utils.GetMonthDifference(dateToCheckLastBill, DateTime.Today) > 0)
                            {
                                d.IsBillDue = true;
                            }
                        }
                        else
                        {
                            d.IsBillDue = true;
                        }
                    }
                    //check the contract validity and if contract is less than a month then check for due bill also
                    if (PassesContractRangeFilters(d, dto))
                        filteredData.Add(d);


                }


                response.Data = filteredData;
                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;

            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;

            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetById([FromBody] ContractFilterDto dto)
        {
            ApiMessage response = new ApiMessage();
            try
            {
                var contract = new Contract();
                var user = new LoggedInUser();

                dto.CompanyId = user.DefaultCompanyId;
                var data = await contract.GetById(dto);
                var jobCard = new JobCard();
                if (data != null)
                {
                    var jobCards = await jobCard.GetList(new JobCardDto
                    {
                        JobCardType = "contract",
                        JobCardTypeKey = dto.ContractId,
                        CompanyId = dto.CompanyId
                    });
                    if (jobCards != null)
                    {
                        data.JobCards = jobCards.ToList();
                    }

                    if (data.QuotationId > 0)
                    {
                        Billing objBilling = new Billing();
                        data.Quotation = await objBilling.QuotationById(data.QuotationId, user.DefaultCompanyId);
                    }
                    if (data.ContractId > 0)
                    {
                        Billing objBilling = new Billing();
                        data.ContractQuotations = objBilling.QuotationsListByContractId(data.ContractId, user.DefaultCompanyId)
                            ?? new List<QuotationDTO>();
                    }
                    else
                    {
                        data.ContractQuotations = new List<QuotationDTO>();
                    }
                }
                response.Data = data;
                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;

            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;

            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetContractDocuments([FromBody] ContractFilterDto dto)
        {
            ApiMessage response = new ApiMessage();
            try
            {
                var contract = new Contract();
                var user = new LoggedInUser();
                dto.CompanyId = user.DefaultCompanyId;
                if (dto.ContractId <= 0)
                {
                    response.Code = ApiMessageCodes.ERROR;
                    response.Message = "Contract id is required.";
                    return Ok(response);
                }

                var data = await contract.GetContractDocumentsAsync(dto);
                response.Data = data != null ? data.ToList() : new List<ContractDocumentDto>();
                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;
            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateStatus([FromBody] ContractUpdateStatusDto dto)
        {
            ApiMessage response = new ApiMessage();
            try
            {
                var contract = new Contract();
                var user = new LoggedInUser();
                var cDto = new ContractDTO();
                cDto.InjectFrom(dto);
                cDto.UpdatedBy = user.UserId;
                cDto.UpdatedOn = DateTime.Now;
                dto.CompanyId = user.DefaultCompanyId;



                var data = await contract.UpdateContractStatus(cDto);

                response.Data = data;
                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;

            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;

            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GenerateBill([FromBody] ContractBillingDto dto)
        {
            ApiMessage response = new ApiMessage();
            try
            {
                var contract = new Contract();
                var user = new LoggedInUser();
                var cDto = new ContractDTO();
                cDto.InjectFrom(dto);
                cDto.UpdatedBy = user.UserId;
                cDto.UpdatedOn = DateTime.Now;
                dto.CompanyId = user.DefaultCompanyId;



                BillingDTO billObj = new BillingDTO();
                Billing objBilling = new Billing();
                //  DataCopier<WorkOrderDTO, WorkOrder> dcopier = new DataCopier<WorkOrderDTO, WorkOrder>();
                // dcopier.CopyData(dto, leder);

                System.Web.HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                JObject jsonObject = new JObject();

                //billObj billObj = new billObj();

                var ct = new Contract();
                var ctData = await ct.GetById(new ContractFilterDto { CompanyId = dto.CompanyId, ContractId = dto.ContractId });

                billObj.ContractId = ctData.ContractId;
                billObj.InvoiceId = dto.InvoiceId;

                billObj.InvoiceDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                //  billObj.From = billObj.To = billObj.InvoiceDate.ToShortDateString();
                billObj.From = billObj.To = billObj.InvoiceDate;

                billObj.LedgerId = ctData.LedgerId;
                billObj.LedgerSiteId = ctData.LedgerSiteId;

                billObj.SubTotal = dto.SubTotal;
                billObj.TaxAmount = dto.IGST + dto.SGST + dto.CGST;
                billObj.FreightTax = 0;

                billObj.FinYearId = new LoggedInUser().FinYearId;
                billObj.Freight = 0;

                billObj.Discount = 0;
                billObj.DiscountPercent = 0;

                billObj.CompanyId = ctData.CompanyId;
                billObj.VoucherNumber = "";
                billObj.CreatedBy = ctData.CreatedBy;
                billObj.InvoiceType = 5; // contract bill
                                         // billObj.SalesDate = DateTime.Today;
                billObj.Remarks = dto.Remarks;
                Sales objSales = new Sales();

                //var taxInfo = jsonObject["AppliedTaxes"];
                billObj.ApplicableTaxes = new List<TaxDTO>();

                if (dto.IGST > 0)
                {
                    billObj.ApplicableTaxes.Add(new TaxDTO
                    {
                        TaxId = 1,
                        TaxAmount = dto.IGST,
                        Rate = dto.IGSTRate
                    });
                }
                if (dto.SGST > 0)
                {
                    billObj.ApplicableTaxes.Add(new TaxDTO
                    {
                        TaxId = 2,
                        TaxAmount = dto.SGST,
                        Rate = dto.SGSTRate
                    });
                }
                if (dto.CGST > 0)
                {
                    billObj.ApplicableTaxes.Add(new TaxDTO
                    {
                        TaxId = 3,
                        TaxAmount = dto.CGST,
                        Rate = dto.CGSTRate
                    });
                }

                billObj.BillableItems = new List<BillingItemDTO>();
                var item = new BillingItemDTO();
                item.LinItem = ctData.Title;
                item.SubTotal = dto.SubTotal;
                item.CreatedBy = user.UserId;
                item.Quantity = 1;
                item.Rate = item.SubTotal;


                billObj.BillableItems.Add(item);
                bool result = await objBilling.CreateInvoice(billObj);

                response.Data = result;
                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;

            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;

            }
            return Ok(response);
        }


        public async Task<ApiMessage> ContractInventory(ContractFilterDto filter)
        {
            ApiMessage response = new ApiMessage();
            try
            {
                var contract = new Contract();
                var user = new LoggedInUser();

                filter.CompanyId = user.DefaultCompanyId;
                var data = await contract.ContractInventory(filter);

                response.Data = data.ToList();
                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;

            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;

            }
            return response;
        }

        public async Task<IHttpActionResult> Extend([FromBody] ContractDTO dto)
        {
            ApiMessage response = new ApiMessage();
            try
            {
                var contract = new Contract();
                var user = new LoggedInUser();
                var cDto = new ContractDTO();
                cDto.InjectFrom(dto);
                cDto.UpdatedBy = user.UserId;
                cDto.UpdatedOn = DateTime.Now;
                cDto.CompanyId = user.DefaultCompanyId;



                var data = await contract.ExtendContract(cDto);

                response.Data = data;
                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;

            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;

            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<ApiMessage> GetBills(ContractFilterDto filter)
        {
            ApiMessage response = new ApiMessage();
            try
            {
                var contract = new Contract();
                var user = new LoggedInUser();

                filter.CompanyId = user.DefaultCompanyId;
                var data = await contract.GetContractBills(filter.CompanyId, filter.ContractId);

                response.Data = data.ToList();
                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;

            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;

            }
            return response;
        }

        [HttpPost]
        public async Task<ApiMessage> AddActivty(ContractActivity activity)
        {
            ApiMessage response = new ApiMessage();
            try
            {
                var contract = new Contract();
                var user = new LoggedInUser();
                activity.CreatedBy = user.UserId;
                activity.CreatedOn = DateTime.Now;
                activity.ModifiedBy = user.UserId;
                activity.ModifiedOn = DateTime.Now;
                activity.GuId = Guid.NewGuid().ToString();
                activity.CompanyId = user.DefaultCompanyId;
                if (activity.ActivityId > 0)
                {
                    response.Data = await contract.CreateActivityAsync(activity);

                }
                else
                    response.Data = await contract.CreateActivityAsync(activity);


                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;

            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;

            }
            return response;
        }
        [HttpPost]
        public async Task<ApiMessage> DeleteActivity(ContractActivity activity)
        {
            ApiMessage response = new ApiMessage();
            try
            {
                var contract = new Contract();
                var user = new LoggedInUser();
                activity.CreatedBy = user.UserId;
                activity.CreatedOn = DateTime.Now;
                activity.ModifiedBy = user.UserId;
                activity.ModifiedOn = DateTime.Now;
                activity.GuId = Guid.NewGuid().ToString();
                activity.CompanyId = user.DefaultCompanyId;

                response.Data = await contract.DeleteActivityAsync(activity);



                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;

            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;

            }
            return response;
        }
        [HttpPost]
        public async Task<ApiMessage> GetAll(ContractActivity activity)
        {
            ApiMessage response = new ApiMessage();
            try
            {
                var contract = new Contract();
                var user = new LoggedInUser();
                activity.CreatedBy = user.UserId;
                activity.CreatedOn = DateTime.Now;
                activity.ModifiedBy = user.UserId;
                activity.ModifiedOn = DateTime.Now;
                activity.GuId = Guid.NewGuid().ToString();
                activity.CompanyId = user.DefaultCompanyId;

                response.Data = await contract.GetAllAsync(activity);


                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;

            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;

            }
            return response;
        }

        [HttpPost]
        public async Task<ApiMessage> ActivityTracker([FromBody] FilterCriteria filter)
        {
            ApiMessage response = new ApiMessage();
            try
            {
                var contract = new Contract();
                var user = new LoggedInUser();

                filter.CompanyId = user.DefaultCompanyId;
                //Employee emp = new Employee();
                //var allEmployees = emp.GetAll(filter.CompanyId);

                var _data = await contract.ActivityTracker(filter);

                response.Data = _data;
                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;

            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;

            }
            return response;
        }
        [HttpPost]
        public async Task<ApiMessage> EmployeeDPR([FromBody] FilterCriteria filter)
        {
            ApiMessage response = new ApiMessage();
            try
            {
                var contract = new Contract();
                var user = new LoggedInUser();

                filter.CompanyId = user.DefaultCompanyId;
                // Employee emp = new Employee();
                //   var allEmployees = emp.GetAll(filter.CompanyId);

                var _data = await contract.EmployeeDPR(filter);
                foreach (var d in _data)
                {
                    double area = 0;
                    if (double.TryParse(d.Area, out area))
                    {
                        d.AreaCovered = Math.Round(area / d.TotalEmployees, MidpointRounding.AwayFromZero);
                    }
                }

                var employeeWise = (from d in _data
                                    group d by d.Employee into g
                                    select new EmployeeDPRDTO
                                    {
                                        Employee = g.Key,
                                        AreaCovered = g.Sum(o => o.AreaCovered)
                                    }); ;

                var jobCardIds = _data.Select(d => d.JobCardId).Where(id => id > 0).Distinct().ToList();
                var docDal = new ContractDocumentDAL();
                var documents = await docDal.ListByJobCardsAsync(filter.CompanyId, jobCardIds);

                response.Data = new { detail = _data, summary = employeeWise, documents = documents.ToList() };
                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;

            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;

            }
            return response;
        }

        [HttpPost]
        public async Task<ApiMessage> ContractRetChallanItems([FromBody] ContractDTO dto)
        {
            ApiMessage response = new ApiMessage();
            try
            {

                var user = new LoggedInUser();
                var jobCard = new Contract();
                jobCard.InjectFrom(dto);

                dto.CompanyId = user.DefaultCompanyId;

                var data = await jobCard.ContractReturnChallanItems(dto.ContractId, dto.CompanyId);

                response.Data = data;
                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;

            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }
        [HttpPost]
        public async Task<ApiMessage> ContractDelChallanItems([FromBody] ContractDTO dto)
        {
            ApiMessage response = new ApiMessage();
            try
            {

                var user = new LoggedInUser();
                var jobCard = new Contract();
                jobCard.InjectFrom(dto);

                dto.CompanyId = user.DefaultCompanyId;

                var data = await jobCard.ContractDelChallanItems(dto.ContractId, dto.CompanyId);

                response.Data = data;
                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;

            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }
    }


}