using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.Objects;
using BAL.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace FarmaAPI.Controllers
{
    [Authorize]
    public class TaxController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage GetChallanTaxes([FromBody] ChallanDTO obj)
        {
            Tax objTax = new Tax();
            List<TaxDTO> taxes = objTax.GetApplicableTaxes(BAL.Enums.TaxItem.Product, obj.ChallanId);
            return Request.CreateResponse(HttpStatusCode.OK, taxes);
        }



        [HttpPost]
        public HttpResponseMessage GetApplicableTaxes([FromBody] TaxDTO dto)
        {
            Tax objTax = new Tax();
            List<TaxDTO> taxes = objTax.GetApplicableTaxes(dto.ItemId, dto.ItemValue);
            return Request.CreateResponse(HttpStatusCode.OK, taxes);
        }

        [HttpPost]
        public HttpResponseMessage SaveTax()
        {
            string dto = System.Web.HttpContext.Current.Request["dto"];


            JObject jsonObject = new JObject();

            BillingDTO billingDTO = new BillingDTO();
            var jArray = JArray.Parse(dto);
            List<TaxDTO> lstTaxes = new List<TaxDTO>();
            foreach (var item in jArray)
            {
                TaxDTO taxDTO = new TaxDTO();
                taxDTO.ItemId = (BAL.Enums.TaxItem)Enum.ToObject(typeof(BAL.Enums.TaxItem), Convert.ToInt16(item["ItemId"]));
                taxDTO.ItemValue = Convert.ToInt16(item["ItemValue"]);
                taxDTO.TaxId = Convert.ToInt16(item["TaxId"]);
                taxDTO.Rate = Convert.ToDouble(item["Rate"]);
                taxDTO.ApplicableTaxId = Convert.ToInt16(item["ApplicableTaxId"]);
                taxDTO.ModifiedBy = new LoggedInUser().UserId;

                lstTaxes.Add(taxDTO);

            }

            Tax objTax = new Tax();
            bool result = objTax.Save(lstTaxes);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }


        [HttpPost]
        public HttpResponseMessage GetAllTaxes([FromBody] TaxDTO obj)
        {
            Tax objTax = new Tax();

            List<TaxDTO> lst = objTax.GetAllTaxes(new LoggedInUser().DefaultCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, lst);
        }

    }
}
