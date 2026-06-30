using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.DTO;
using BAL.Objects;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
namespace FarmaAPI.Controllers
{
     [Authorize]
    public class InvoiceController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Add()
        {
            bool result = false;
            try
            {
                string dto = System.Web.HttpContext.Current.Request["dto"];
                WorkOrderDTO objWOrder = new WorkOrderDTO();
                Invoice objInvoice = new Invoice();
                JObject jsonObject = JObject.Parse(dto);
                objWOrder.InvoiceNumber = Convert.ToString(jsonObject["InvoiceNumber"]);
                objWOrder.CompanyId = Convert.ToInt16(jsonObject["CompanyId"]);
                objWOrder.SiteId = Convert.ToInt32(jsonObject["SiteId"]);
                objWOrder.InvoiceDate = Convert.ToDateTime(jsonObject["InvoiceDate"]);
                objWOrder.Details = Convert.ToString(jsonObject["Details"]);
                objWOrder.SubTotal = Convert.ToDouble(jsonObject["SubTotal"]);
                objWOrder.CreatedBy = 1;


                var items = jsonObject["Items"];
                List<WorkOrderItemDTO> wItems = new List<WorkOrderItemDTO>();
                foreach (var i in items)
                {
                    var itemObj = i["Item"];
                    if (itemObj == null) continue;
                    wItems.Add(new WorkOrderItemDTO
                    {
                        ProductId = Convert.ToInt32(itemObj["ProductId"]),
                        Rate = Convert.ToDouble(i["Rate"]),
                        PurchaseQty = Convert.ToDouble(i["PurchaseQty"])
                    });
                }
                var taxInfo = jsonObject["Taxes"];
                objWOrder.ApplicableTaxes = new List<TaxDTO>();
                foreach (var tax in taxInfo)
                {
                    TaxDTO taxDto = new TaxDTO();
                    taxDto.TaxId = Convert.ToInt16(tax["TaxId"]);
                    taxDto.Rate = Convert.ToDouble(tax["Rate"]);
                    bool applicable = Convert.ToBoolean(tax["Applicable"]);
                    taxDto.Amount = objWOrder.SubTotal;
                    if (applicable)
                        objWOrder.ApplicableTaxes.Add(taxDto);
                }
                objWOrder.Items = wItems;
                result = objInvoice.Add(objWOrder);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        [HttpPost]
        public HttpResponseMessage GetList(CompanyDTO dto)
        {
            Invoice objInvoice = new Invoice();
            return Request.CreateResponse(HttpStatusCode.OK, objInvoice.InvoiceList(dto.CompanyId));

        }
    }
}
