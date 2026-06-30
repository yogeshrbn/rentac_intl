using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.DTO;
using BAL.Objects;
namespace FarmaAPI.Controllers
{
    [Authorize]
    public class SaltController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Save([FromBody] SaltDTO obj)
        {
            Salt objSalt = new Salt();
            objSalt.Name = obj.Name;
            objSalt.Dosage = obj.Dosage;
            objSalt.CreatedBy = obj.CreatedBy;
            objSalt.DrugInstructions = obj.DrugInstructions;
            objSalt.Indications = obj.Indications;
            objSalt.Narcotic = obj.Narcotic;
            objSalt.Note = obj.Note;
            objSalt.Precautions = obj.Precautions;
            objSalt.SaltId = obj.SaltId;
            objSalt.SCH_H = obj.SCH_H;
            objSalt.SCH_H1 = obj.SCH_H1;
            objSalt.SideEffects = obj.SideEffects;
            objSalt.Save();
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }
        [HttpPost]
        public HttpResponseMessage GetAll([FromBody] SaltDTO obj)
        {
            Salt objSalt = new Salt();
            List<SaltDTO> lstSalts = objSalt.GetAll(obj.StoreId);
            return Request.CreateResponse(HttpStatusCode.OK, lstSalts);
        }
        [HttpPost]
        public HttpResponseMessage GetInfo([FromBody] SaltDTO obj)
        {
            obj = new Salt(obj.SaltId);
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }

        [HttpPost]
        public HttpResponseMessage ChangeStatus([FromBody] SaltDTO obj)
        {
            Salt objNew = new Salt(obj.SaltId);
            objNew.Status = obj.Status;
            objNew.ChangeStatus();
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
