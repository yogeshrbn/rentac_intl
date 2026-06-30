using BAL.DTO;
using BAL.Objects;
using FarmaAPI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FarmaAPI.Controllers
{
    public class ClientController : BaseApiController
    {
        [HttpGet]
        public ApiMessage GetInfo()
        {
            var message = new ApiMessage();
            var client = new RBNClient();
            var clientInfo = client.GetInfo(new LoggedInUser().RbnClientId);

            message.Data = clientInfo;
            message.Code = ApiMessageCodes.SUCCESS;
            return message;
        }

        [HttpPost]
        public ApiMessage UpdateInfo([FromBody] RBNClientDTO dto)
        {
            var message = new ApiMessage();
            var client = new RBNClient();
            if (dto.NoGst == null)
            {
                dto.NoGst = 0;
            }
            dto.RbnClientId = new LoggedInUser().RbnClientId;
            var clientInfo = client.UpdateInfo(dto);
            message.Data = clientInfo;
            message.Code = ApiMessageCodes.SUCCESS;
            return message;
        }

        //[HttpPost]
        //public ApiMessage UpdateTaxInfo([FromBody] RBNClientDTO dto)
        //{
        //    var message = new ApiMessage();
        //    var client = new RBNClient();
        //    dto.RbnClientId = new LoggedInUser().RbnClientId;
        //    var clientInfo = client.UpdateTaxInfo(dto);
        //    message.Data = clientInfo;
        //    message.Code = ApiMessageCodes.SUCCESS;
        //    return message;
        //}
    }
}
