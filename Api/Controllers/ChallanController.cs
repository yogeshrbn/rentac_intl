using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.DTO;
using BAL.Objects;
using BAL.Common;
using FarmaAPI.Helper;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
namespace FarmaAPI.Controllers
{
    [Authorize]
    public class ChallanController : ApiController
    {
        /// <summary>
        ///Creates new lift material delivery challan
        /// </summary>
        /// <param name="dto">AccountGroupDTO</param>
        /// <returns>true/false</returns>
        [HttpPost]
        public HttpResponseMessage SaveChallan()
        {
            WorkOrder wOrder = new WorkOrder(0);
            // Challan leder = new Challan(0);
            //DataCopier<ChallanDTO, Challan> dcopier = new DataCopier<ChallanDTO, Challan>();
            //dcopier.CopyData(dto, leder);
            JObject jsonObject = new JObject();
            string dto = System.Web.HttpContext.Current.Request["dto"];
            System.Web.HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            int workOrderId;
            try
            {

                jsonObject = JObject.Parse(dto);
                NextId n = new NextId();
                string chNumber = "0";


                //ParentWorkOrderId will be needed if sending items on the same site from siteInfoPage.
                wOrder.ParentWorkOrderId = Convert.ToInt32(jsonObject.GetValue("ParentWorkOrderId"));
                wOrder.WorkOrderId = Convert.ToInt32(jsonObject.GetValue("WorkOrderId"));
                if (wOrder.WorkOrderId == 0)
                {
                    chNumber = n.GetNextId(BAL.Enums.NextIDTables.Challan, new LoggedInUser().FinYearId.ToString(), new LoggedInUser().DefaultCompanyId);
                }
                else
                {
                    if (jsonObject["ChallanNumber"] != null)
                    {
                        chNumber = Convert.ToString(jsonObject["ChallanNumber"]);
                    }
                }
                // wOrder.ChallanNumber = chNumber;// Convert.ToString(jsonObject.GetValue("Challanumber"));
                wOrder.CompanyId = new LoggedInUser().DefaultCompanyId;
                wOrder.LedgerId = Convert.ToInt32(jsonObject.GetValue("LedgerId"));
                //  wOrder.ChallanDate = Utils.FormatDate(Convert.ToString(jsonObject.GetValue("Date")));
                wOrder.WorkOrderDate = Utils.FormatDate(Convert.ToString(jsonObject.GetValue("Date")));
                wOrder.Number = chNumber;
                wOrder.WorkOrderNumber = chNumber;
                wOrder.RbnClientId = new LoggedInUser().RbnClientId;
                wOrder.FinYearId = new LoggedInUser().FinYearId;
                wOrder.ChallanType = BAL.Enums.ChallanType.LIFT_DELIVERY;
                wOrder.CreatedBy = new LoggedInUser().UserId;

                SiteDTO siteDto = new SiteDTO();
                siteDto.ChallanNumber = chNumber;
                siteDto.StartDate = Utils.FormatDate(Convert.ToString(jsonObject.GetValue("Date")));
                siteDto.Site = Convert.ToString(jsonObject.GetValue("Site"));
                siteDto.Vehicle = Convert.ToString(jsonObject.GetValue("Vehicle"));
                siteDto.Driver = Convert.ToString(jsonObject.GetValue("Driver"));
                siteDto.Freight = Convert.ToDouble(jsonObject.GetValue("Freight"));
                siteDto.SubTotal = Convert.ToDouble(jsonObject.GetValue("SubTotal"));
                siteDto.State = Convert.ToString(jsonObject.GetValue("State"));

                if (jsonObject["SiteId"] != null)
                {
                    siteDto.SiteId = Convert.ToInt32(jsonObject.GetValue("SiteId"));
                }

                if (jsonObject.GetValue("Number") != null)
                {
                    siteDto.JobNumber = Convert.ToString(jsonObject.GetValue("Number"));
                }
                siteDto.Total = (siteDto.SubTotal + siteDto.Freight);
                siteDto.Items = new List<WorkOrderItemDTO>();
                var items = jsonObject["Items"];
                siteDto.Taxes = new List<TaxDTO>();
                foreach (var item in items)
                {
                    WorkOrderItemDTO chItem = new WorkOrderItemDTO();
                    chItem.ProductId = Convert.ToInt32(item["ProductId"]);
                    chItem.PurchaseQty = Convert.ToInt32(item["Quantity"]);
                    if (item["ProductSizeId"] != null)
                    {
                        chItem.ProductSizeId = Convert.ToInt32(item["ProductSizeId"]);
                    }
                    if (item["WorkOrderItemId"] != null)
                    {
                        chItem.WorkOrderItemId = Convert.ToInt32(item["WorkOrderItemId"]);
                    }
                    siteDto.Items.Add(chItem);
                }
                wOrder.Sites.Add(siteDto);

                workOrderId = wOrder.Save();
                //  wOrder.AddSite(wOrder.Sites, 0);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, workOrderId);
        }

        [HttpPost]
        public HttpResponseMessage GetChallanList([FromBody] FilterCriteria dto)
        {
            try
            {
                Challan challan = new Challan(0);

                dto.LedgerId = dto.LedgerId;
                string from = Utils.FormatDate(dto.From).ToShortDateString();
                string to = Utils.FormatDate(dto.To).ToShortDateString();
                List<WorkOrderDTO> challans = challan.GetChallanList(dto.LedgerId,new LoggedInUser().DefaultCompanyId, from, to, BAL.Enums.ChallanType.LIFT_DELIVERY);
                return Request.CreateResponse(HttpStatusCode.OK, challans);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage AddChallanToSite([FromBody] WorkOrderDTO dto)
        {
            try
            {
                Challan challan = new Challan(0);

                dto.LedgerId = dto.LedgerId;
                int return_value = challan.AddChallanToSite(new LoggedInUser().DefaultCompanyId, dto.WorkOrderId, dto.Number);

                return Request.CreateResponse(HttpStatusCode.OK, return_value);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage GetSiteChallans([FromBody] WorkOrderDTO dto)
        {
            try
            {
                Challan challan = new Challan(0);

                return Request.CreateResponse(HttpStatusCode.OK, challan.GetSiteChallans(dto.WorkOrderId));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
