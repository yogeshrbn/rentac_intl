using BAL.DTO;
using BAL.Enums;
using BAL.Objects;
using BAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace ReportViewer.Controllers
{
    [CustomActionFilter]
    [System.Web.Mvc.Authorize]
    public class EmailController : Controller
    {
        // GET: Email
        NotificationService service = new NotificationService();
        public ActionResult Index()
        {
            return View();
        }

        [System.Web.Mvc.HttpPost]

        public async Task<bool> SendEmail([FromBody] NotificationDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return true;
                }

                var noticationService = new NotificationService();

                var user = new LoggedInUser();
                dto.CompanyId = user.DefaultCompanyId;
                var company = new Company(dto.CompanyId);
                var cDto = company.GetDetails();
                dto.CreatedBy = user.UserId;
                dto.FinYearId = user.FinYearId;
                dto.Sender = cDto.Email;
                dto.CreatedOn = DateTime.Now;
                dto.GuId = Guid.NewGuid().ToString();
                dto.Type = "email";
                dto.Category = "email";
                var result = await service.Add(dto);
                if (result == true)
                {
                    string dwnFileName;
                    byte[] fileBytes;
                    var pdfGen = new PDFGenerator();
                    var strParams = dto.MetaData.Split(',');


                    pdfGen.GenerateQuotationPdf(Convert.ToInt32(strParams[1]), out dwnFileName, out fileBytes);
                    dto.AttachmentDocs = new List<AttachmentDoc>();
                    dto.AttachmentDocs.Add(new AttachmentDoc
                    {
                        Buffer = fileBytes,
                        Name = strParams[2],
                        ContentType = "application/octet-stream"
                    });
                    result = await service.Send(dto);
                }
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
            //if (result == true)
            //{
            //    string file = Encoding.UTF8.GetString(Convert.FromBase64String(dto.MetaData));

            //    var dec = Security.Decrypt(file);

            //    var strParams = dec.Split(',');
            //    var repName = strParams[0];

            //    string dwnFileName;
            //    byte[] fileBytes;
            //    var pdfGen = new PDFGenerator();
            //    pdfGen.GenerateQuotationPdf(strParams, out dwnFileName, out fileBytes);


            //}
        }


    }
}