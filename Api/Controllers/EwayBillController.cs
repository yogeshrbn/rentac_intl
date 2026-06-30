using BAL.Common;
using BAL.DTO;
using BAL.Exceptions;
using BAL.Objects;
using FarmaAPI.Helper;
using Omu.ValueInjecter;
using QRCoder;
using Spire.Pdf.General.Paper.Uof;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http;
using System.Windows;
namespace FarmaAPI.Controllers
{
    [Authorize]
    public class EwayBillController : BaseApiController
    {
        [HttpPost]
        public async Task<ApiMessage> Save([FromBody] EwayBillDTO obj)
        {
            var msg = new ApiMessage();
            try
            {
                if (obj == null)
                {
                    throw new Exception("Please provide valid input");
                }
                var user = new LoggedInUser();
                TransporterService objService = new TransporterService();
                var list = objService.GetAll(new TransporterDTO { CompanyId = user.DefaultCompanyId });
                var selectedTp = list.Where(o => o.TransporterId == obj.TransporterId).FirstOrDefault();

                if (selectedTp == null)
                {
                    // throw new Exception("Please select transporter");
                }
                if (selectedTp != null)
                {
                    obj.TransporterName = selectedTp.Name;
                    obj.TransporterGST = selectedTp.GST;
                }


                obj.CreatedBy = user.UserId;
                obj.CompanyId = user.DefaultCompanyId;
                obj.CreatedOn = DateTime.Now;
                obj.GuId = Guid.NewGuid().ToString();

                var bill = new Billing();


                //if (result)
                //{
                //TransporterService objService = new TransporterService();
                //var list = objService.GetAll(new TransporterDTO { CompanyId = user.DefaultCompanyId });
                //var selectedTp = list.Where(o => o.TransporterId == obj.TransporterId).FirstOrDefault();
                //if (selectedTp == null)
                //{
                //    throw new Exception("Please select transporter");
                //}


                var buyer = new EwayBillPartyDto();
                var seller = new EwayBillPartyDto();

                var billingData = new BillingDTO();
                if (obj.DocType.ToLower() == "inv")
                {

                    billingData = bill.GetBillingInfo(obj.InvoiceId);
                    billingData.BillableItems = obj.Items;// bill.BillItems(obj.InvoiceId);
                    obj.Buyer = getPartyDto(billingData.LedgerId, billingData.LedgerSiteId);
                    obj.Seller = getCompanyPartyDto(obj.CompanyId);

                }
                if (obj.DocType.ToLower() == "chl" && obj.DocSubType == "del")
                {
                    var wo = new WorkOrder(obj.InvoiceId);

                    //    var sites = wo.GetSites(obj.InvoiceId);
                    //var ledger = new Ledger(wo.LedgerId);
                    //var lederDetails = ledger.GetDetails();
                    //var siteInfo = ledger.GetSiteById(wo.LedgerSiteId);
                    billingData.InjectFrom(wo);
                    billingData.InvoiceDate = wo.WorkOrderDate;
                    billingData.InvoiceNumber = wo.Number;
                    billingData.Total = obj.Items.Sum(o => o.SubTotal);
                    //obj.ShipToGST = siteInfo.GSTNo;
                    //   billingData.BillableItems = new List<BillingItemDTO>();
                    billingData.BillableItems = obj.Items;

                    obj.Buyer = getPartyDto(wo.LedgerId, wo.LedgerSiteId);
                    obj.Seller = getCompanyPartyDto(obj.CompanyId);
                }


                if (obj.DocType.ToLower() == "chl" && obj.DocSubType == "ret")
                {
                    GRN objGrn = new GRN();
                    var grn = await objGrn.GrnById(obj.InvoiceId, user.DefaultCompanyId);

                    //    var sites = wo.GetSites(obj.InvoiceId);
                    var ledger = new Ledger();
                    var siteInfo = ledger.GetSiteById(grn.LedgerSiteId);
                    billingData.InjectFrom(grn);
                    billingData.InvoiceDate = grn.ReceivingDate;
                    billingData.InvoiceNumber = grn.GRN;
                    billingData.Total = obj.Items.Sum(o => o.SubTotal);
                    obj.ShipToGST = siteInfo.SiteGST;
                    //   billingData.BillableItems = new List<BillingItemDTO>();
                    billingData.BillableItems = obj.Items;

                    obj.Buyer = getCompanyPartyDto(obj.CompanyId);
                    obj.Seller = getPartyDto(grn.LedgerId, grn.LedgerSiteId);
                }

                if (obj.Buyer == null)
                {
                    throw new Exception("Buyer information is empty");
                }
                if (obj.Seller == null)
                {
                    throw new Exception("Seller information is empty");
                }

                obj.Buyer.Address = obj.ShipToAddress;
                obj.Buyer.ZipCode = obj.ShipToZipCode;
                obj.Buyer.City = obj.ShipToCity;
                obj.Buyer.StateCode = obj.ShipToStateCode;

                obj.Seller.Address = obj.ShipFromAddress;
                obj.Seller.ZipCode = obj.ShipFromZipCode;
                obj.Seller.City = obj.ShipFromCity;
                obj.Seller.StateCode = obj.ShipFromStateCode;


                if (String.IsNullOrEmpty(obj.Buyer.GST) || String.IsNullOrEmpty(obj.Buyer.Address) || String.IsNullOrEmpty(obj.Buyer.ZipCode)
                    || String.IsNullOrEmpty(obj.Buyer.City)
                    )
                {
                    throw new Exception("Please check buyer GST/Address/City/PinCode. All these information must not be empty");
                }
                if (String.IsNullOrEmpty(obj.Buyer.StateCode))
                {
                    throw new Exception("Please check buyer state code.");
                }
                if (String.IsNullOrEmpty(obj.Buyer.TradeName))
                {
                    throw new Exception("Please check buyer trade name");
                }

                if (String.IsNullOrEmpty(obj.Seller.GST) || String.IsNullOrEmpty(obj.Seller.Address) ||
                    String.IsNullOrEmpty(obj.Seller.ZipCode) || String.IsNullOrEmpty(obj.Seller.City))
                {
                    throw new Exception("Please check seller GST/Address/City/PinCode. All these information must not be empty");
                }
                if (String.IsNullOrEmpty(obj.Seller.StateCode))
                {
                    throw new Exception("Please check seller state code.");
                }
                if (String.IsNullOrEmpty(obj.Seller.TradeName))
                {
                    throw new Exception("Please check seller trade name");
                }



                var result = await bill.AddEwayBill(obj);
                if (result)
                {
                    var userInfo = new LoggedInUserInfo();
                    userInfo.InjectFrom(user);
                    var eWayService = new EwayBillService();
                    result = await eWayService.CreateEwayBill(billingData, userInfo, obj);
                }
                //}

                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EwayBillPartyDto getCompanyPartyDto(int companyId)
        {
            var party = new EwayBillPartyDto();
            var comp = new Company(companyId).GetDetails();
            party.Id = comp.CompanyId;
            party.Address = String.Concat(comp.Address1, " ", comp.Address2, " ", comp.City);
            party.ZipCode = comp.ZipCode;
            party.StateCode = Convert.ToString(comp.StateCode);
            party.StateId = comp.StateId;
            party.GST = comp.GSTNo;
            party.City = comp.City;
            party.TradeName = comp.TradeName;
            return party;
        }
        [HttpPost]
        public EwayBillPartyDto getPartyInfo([FromBody] FilterCriteria filter)
        {
            try
            {
                if (filter == null)
                {
                    throw new Exception("Invalid of empty filter");
                }
                return getPartyDto(filter.LedgerId, filter.LedgerSiteId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EwayBillPartyDto getPartyDto(int lederId, int lederSiteId)
        {
            var user = new LoggedInUser();
            var party = new EwayBillPartyDto();
            var siteInfo = new ClientSiteDTO();
            var ledger = new Ledger(lederId);
           
            var lederDetails = ledger.GetDetails();
            party.Id = lederDetails.LedgerId;
            if (lederDetails.CompanyId != user.DefaultCompanyId)
            {
                throw new Exception("Eway bill company and curent user's logged in company not matching");
            }
            if (lederSiteId > 0)
            {
                siteInfo = ledger.GetSiteById(lederSiteId);
            }
            if (siteInfo != null)
            {
                party.Address = String.Concat(siteInfo.SiteAddress, " ", siteInfo.Address2, " ", siteInfo.City);
                party.City = siteInfo.City;
                party.ZipCode = siteInfo.ZipCode;
                party.StateCode = Convert.ToString(siteInfo.StateCode);
                party.StateId = siteInfo.StateId;
            }
            if (siteInfo != null && !String.IsNullOrEmpty(siteInfo.SiteGST))
            {
                party.GST = siteInfo.SiteGST;
            }
            else
            {
                party.GST = lederDetails.GSTNo;
            }
            if (siteInfo == null)
            {
                party.Address = String.Concat(lederDetails.ShipAddress1, " ", lederDetails.ShipAddress2, " ", lederDetails.ShipCity);
                party.ZipCode = lederDetails.ShipZipCode;
                party.City = lederDetails.ShipCity;
                party.StateCode = lederDetails.ShipStateCode;
                party.StateId = lederDetails.ShipStateId;
            }
            party.TradeName = lederDetails.TradeName;
            return party;
        }


        [HttpPost]
        public async Task<ApiMessage> PushToPortal([FromBody] EwayBillDTO obj)
        {
            var msg = new ApiMessage();
            try
            {
                if (obj == null)
                {
                    throw new Exception("Please provide valid input");
                }
                if (obj.EwayBillId == 0)
                {
                    throw new Exception("Please provide valid input");
                }

                var user = new LoggedInUser();

                var bill = new Billing();
                var objEBill = await bill.GetEwayBill(obj.EwayBillId, user.DefaultCompanyId);
                var billingData = new BillingDTO();
                if (obj.DocType.ToLower() == "inv")
                {

                    billingData = bill.GetBillingInfo(objEBill.InvoiceId);
                    billingData.BillableItems = objEBill.Items;// bill.BillItems(obj.InvoiceId);
                    objEBill.Buyer = getPartyDto(billingData.LedgerId, billingData.LedgerSiteId);
                    objEBill.Seller = getCompanyPartyDto(objEBill.CompanyId);

                }
                if (objEBill.DocType.ToLower() == "chl" && objEBill.DocSubType == "del")
                {
                    var wo = new WorkOrder(objEBill.InvoiceId);

                    //    var sites = wo.GetSites(obj.InvoiceId);
                    //var ledger = new Ledger(wo.LedgerId);
                    //var lederDetails = ledger.GetDetails();
                    //var siteInfo = ledger.GetSiteById(wo.LedgerSiteId);
                    billingData.InjectFrom(wo);
                    billingData.InvoiceDate = wo.WorkOrderDate;
                    billingData.InvoiceNumber = wo.Number;
                    billingData.Total = objEBill.Items.Sum(o => o.SubTotal);
                    //obj.ShipToGST = siteInfo.GSTNo;
                    //   billingData.BillableItems = new List<BillingItemDTO>();
                    billingData.BillableItems = objEBill.Items;

                    objEBill.Buyer = getPartyDto(wo.LedgerId, wo.LedgerSiteId);
                    objEBill.Seller = getCompanyPartyDto(objEBill.CompanyId);




                }


                if (obj.DocType.ToLower() == "chl" && obj.DocSubType == "ret")
                {
                    GRN objGrn = new GRN();
                    var grn = await objGrn.GrnById(objEBill.InvoiceId, user.DefaultCompanyId);

                    //    var sites = wo.GetSites(obj.InvoiceId);
                    var ledger = new Ledger();
                    var siteInfo = ledger.GetSiteById(grn.LedgerSiteId);
                    billingData.InjectFrom(grn);
                    billingData.InvoiceDate = grn.ReceivingDate;
                    billingData.InvoiceNumber = grn.GRN;
                    billingData.Total = objEBill.Items.Sum(o => o.SubTotal);
                    objEBill.ShipToGST = siteInfo.SiteGST;
                    //   billingData.BillableItems = new List<BillingItemDTO>();
                    billingData.BillableItems = objEBill.Items;

                    objEBill.Buyer = getCompanyPartyDto(obj.CompanyId);
                    objEBill.Seller = getPartyDto(grn.LedgerId, grn.LedgerSiteId);
                }




                bool result = false;
                if (objEBill != null)
                {


                    //var billingData = bill.GetBillingInfo(obj.InvoiceId);
                    //billingData.BillableItems = bill.BillItems(obj.InvoiceId);
                    var userInfo = new LoggedInUserInfo();
                    userInfo.InjectFrom(user);
                    var eWayService = new EwayBillService();
                    result = await eWayService.CreateEwayBill(billingData, userInfo, objEBill);
                }

                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public async Task<ApiMessage> UpdateVehicle([FromBody] UdpateVehicleDto obj)
        {
            var msg = new ApiMessage();
            try
            {
                if (obj == null)
                {
                    throw new Exception("Please provide valid input");
                }
                var user = new LoggedInUser();
                obj.CreatedBy = user.UserId;
                obj.CompanyId = user.DefaultCompanyId;
                //obj.CreatedOn = DateTime.Now;
                //obj.GuId = Guid.NewGuid().ToString();

                //var bill = new Billing();
                //var result = bill.AddEwayBill(obj);

                if (obj.TransportationMode <= 0 || obj.FromStateId <= 0 ||
                    String.IsNullOrEmpty(obj.FromPlace) || String.IsNullOrEmpty(obj.ReasonCode) ||
                     String.IsNullOrEmpty(obj.VehicleNo) || String.IsNullOrEmpty(obj.EwayBillNo))
                {
                    throw new UDFException("Invalid details provided to udpate the vehicle information", ErrorCodes.INVALID_INFO_FOR_VEHICLE_UPDATE);
                }



                //if (result)
                //{
                var userInfo = new LoggedInUserInfo();
                userInfo.InjectFrom(user);
                var eWayService = new EwayBillService();






                var result = await eWayService.UpdateVehileInfo(userInfo, obj);
                // }

                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<ApiMessage> UpdateTransporter([FromBody] UdpateVehicleDto obj)
        {
            var msg = new ApiMessage();
            try
            {
                if (obj == null)
                {
                    throw new Exception("Please provide valid input");
                }
                var user = new LoggedInUser();
                obj.CreatedBy = user.UserId;
                obj.CompanyId = user.DefaultCompanyId;
                //obj.CreatedOn = DateTime.Now;
                //obj.GuId = Guid.NewGuid().ToString();

                //var bill = new Billing();
                //var result = bill.AddEwayBill(obj);

                if (String.IsNullOrEmpty(obj.TransporterGST) || String.IsNullOrEmpty(obj.EwayBillNo))
                {
                    throw new UDFException("Invalid details provided to udpate the transporter information", ErrorCodes.INVALID_INFO_FOR_TRANSPORTER_UPDATE);
                }



                //if (result)
                //{
                var userInfo = new LoggedInUserInfo();
                userInfo.InjectFrom(user);
                var eWayService = new EwayBillService();






                var result = await eWayService.UpdateTransporter(userInfo, obj);
                // }

                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<ApiMessage> CancelEwayBill([FromBody] CancelEwayBillDto obj)
        {
            var msg = new ApiMessage();
            try
            {
                if (obj == null)
                {
                    throw new Exception("Please provide valid input");
                }
                var user = new LoggedInUser();
                obj.CreatedBy = user.UserId;
                obj.CompanyId = user.DefaultCompanyId;
                //obj.CreatedOn = DateTime.Now;
                //obj.GuId = Guid.NewGuid().ToString();

                //var bill = new Billing();
                //var result = bill.AddEwayBill(obj);

                if (obj.CancelReasonCode <= 0 || String.IsNullOrEmpty(obj.EwayBillNo))
                {
                    throw new UDFException("Invalid details provided to Cancel the bill", ErrorCodes.INVALID_INFO_FOR_BILL_CANCEL);
                }

                var userInfo = new LoggedInUserInfo();
                userInfo.InjectFrom(user);
                var eWayService = new EwayBillService();

                var result = await eWayService.CancelEwayBill(userInfo, obj);
                if (result)
                {
                    var billing = new Billing();
                    obj.CancelledBy = user.UserId;
                    obj.CancelledOn = DateTime.Now;

                    result = billing.CancelEwayBIll(obj);
                }

                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ApiMessage GetAll([FromBody] EwayBillFilterDto filter)
        {
            var msg = new ApiMessage();
            try
            {

                var user = new LoggedInUser();

                filter.CompanyId = user.DefaultCompanyId;

                var bill = new Billing();
                msg.Data = bill.GetAllEwayBills(filter);

                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public HttpResponseMessage PrintBill([FromBody] EwayBillDTO dto)
        {
            try
            {

                Billing billing = new Billing();
                DataSet ds = billing.PrintEwayBill(dto);
                int partyId = 0;

                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        partyId = Convert.ToInt32(ds.Tables[0].Rows[0]["LedgerId"]);


                        var qrCodeStr = Convert.ToString(ds.Tables[0].Rows[0]["ewayBillNo"]);

                        var codeGen = new QRCodeGenerator();

                        var qrCodeData = codeGen.CreateQrCode(Encoding.UTF8.GetBytes(qrCodeStr), QRCodeGenerator.ECCLevel.Q);
                        using (Base64QRCode qrCode = new Base64QRCode(qrCodeData))
                        {


                            ds.Tables[0].Rows[0]["qrCode"] = qrCode.GetGraphic(50);// Convert.ToBase64String(qrCodeImage);
                        }


                    }
                }
                String reportFileName = "e-waybill.rdlc";

                BAL.Objects.Report objRPT = new BAL.Objects.Report();


                List<string> files = new List<string>();


                string pdfName = ds.Tables[0].Rows[0]["ewayBillNo"].ToString();

                //  DataSet headerDataSet = objRPT.GetReportHeader_Bill(partyId, new LoggedInUser().DefaultCompanyId, dto.InvoiceId, x);
                string fileName = CreateReportFile(pdfName, reportFileName, null, ds, BAL.Enums.ExportFormat.PDF);
                files.Add(fileName);
                String outPutFIle = fileName;
                if (files.Count > 1)
                {
                    outPutFIle = MergeFiles(files, Guid.NewGuid().ToString() + ".pdf");
                }
                return Request.CreateResponse(HttpStatusCode.OK, outPutFIle);

            }
            catch (Exception ex)
            {
                String msg = ex.Message;
                if (ex.InnerException != null)
                {
                    msg += ex.InnerException.Message;
                }
                return Request.CreateResponse(HttpStatusCode.OK, msg);
            }
        }
        [HttpPost]
        public async Task<ApiMessage> GetInfo([FromBody] EwayBillDTO obj)
        {
            var bill = new Billing();
            var msg = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                if (obj == null)
                {
                    throw new ArgumentNullException(nameof(obj));
                }
                if (obj.EwayBillId <= 0)
                {
                    throw new ArgumentNullException("EwayBillId");
                }
                msg.Data = await bill.GetEwayBill(obj.EwayBillId, user.DefaultCompanyId);
                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<ApiMessage> FetchEwayBillsByDateFromPortal([FromBody] EwayBillDTO obj)
        {
            var msg = new ApiMessage();
            try
            {
                if (obj == null)
                {
                    throw new Exception("Please provide valid input");
                }
                var user = new LoggedInUser();
                obj.CreatedBy = user.UserId;
                obj.CompanyId = user.DefaultCompanyId;


                if (obj.CreatedOn.Year <= 2000)
                {
                    throw new UDFException("Invalid details provided to Cancel the bill", ErrorCodes.INVALID_INFO_FOR_BILL_CANCEL);
                }
                if (obj.CreatedOn > DateTime.Today)
                {
                    throw new UDFException("To Date must be less than today", ErrorCodes.INVALID_INFO_FOR_BILL_CANCEL);
                }
                var userInfo = new LoggedInUserInfo();
                userInfo.InjectFrom(user);
                var eWayService = new EwayBillService();

                var result = await eWayService.FetchEwayBillsByDate(userInfo, obj);
                msg.Data = result;

                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ApiMessage> MapEwayBill([FromBody] EwayBillDTO obj)
        {
            var msg = new ApiMessage();
            try
            {
                if (obj == null)
                {
                    throw new Exception("Please provide valid input");
                }
                var user = new LoggedInUser();
                obj.CreatedBy = user.UserId;
                obj.CompanyId = user.DefaultCompanyId;
                var billing = new Billing();


                var userInfo = new LoggedInUserInfo();
                userInfo.InjectFrom(user);


                var result = await billing.GetEwayBillByDocNumber(obj);
                var totalRows = result.Count();
                if (totalRows == 0)
                {
                    throw new Exception("Could not find any matching record");
                }
                if (totalRows > 1)
                {
                    msg.Data = result;
                }
                if (totalRows == 1)
                {
                    var ewayService = new EwayBillService();
                    var dto = result.ToList()[0];

                    dto.EwayBillNo = obj.EwayBillNo;
                    dto.EwayBillDate = obj.EwayBillDate;
                    dto.EwayBillValidUpTo = obj.EwayBillValidUpTo;
                    dto.EwayBillAlert = obj.EwayBillAlert;
                    dto.EwayBillCreatedBy = user.UserId;


                    msg.Data = ewayService.UpdateEwayBillInfo(dto);
                    //map the rows
                }

                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Updates the ewaybill info locally. This is being called from UI when user select a local bill/challan to update with portal details.
        /// This situation will come if there are multiple eway bills found with same document number.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiMessage> UpdateEwayBillInfo([FromBody] EwayBillDTO obj)
        {
            var msg = new ApiMessage();
            try
            {
                if (obj == null)
                {
                    throw new Exception("Please provide valid input");
                }
                var user = new LoggedInUser();
                obj.CreatedBy = user.UserId;
                obj.CompanyId = user.DefaultCompanyId;
                var billing = new EwayBillService();

                var userInfo = new LoggedInUserInfo();
                userInfo.InjectFrom(user);

                var result = billing.UpdateEwayBillInfo(obj);
                msg.Data = result;

                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
