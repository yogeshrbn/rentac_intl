using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.Objects;
using BAL.DTO;
using FarmaAPI.Helper;

using System.Web.Script.Serialization;
using BAL.Exceptions;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Runtime;
using System.Web.Http.Description;
using System.IO;
using Microsoft.Ajax.Utilities;
using System.Web;
using NLog;

namespace FarmaAPI.Controllers
{
    [Authorize]
    public class CompanyController : ApiController

    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string mapPath = Path.Combine(HttpRuntime.AppDomainAppPath, "docs");

        [HttpPost]
        public HttpResponseMessage Save([FromBody] CompanyDTO obj)
        {
            var msg = new ApiMessage();
            try
            {

                Company objCompany = new Company(obj.CompanyId);
                objCompany.Name = obj.Name;
                objCompany.Address1 = obj.Address1;
                objCompany.Address2 = obj.Address2;
                objCompany.Email = obj.Email;
                objCompany.Phone1 = obj.Phone1;
                objCompany.Phone2 = obj.Phone2;
                objCompany.Contact = obj.Contact;
                //   objCompany.Fax = obj.Fax;
                objCompany.Web = obj.Web;
                objCompany.City = obj.City;
                objCompany.State = obj.State;
                objCompany.ZipCode = obj.ZipCode;
                objCompany.TIN = obj.TIN;
                objCompany.TAN = obj.TAN;
                objCompany.PAN = obj.PAN;
                objCompany.SignAuthority = obj.SignAuthority;
                objCompany.ReportHeader = obj.ReportHeader;
                objCompany.ParentCompanyId = obj.ParentCompanyId;
                objCompany.GSTNo = obj.GSTNo;
                objCompany.RbnClientId = new LoggedInUser().RbnClientId;
                objCompany.StateId = obj.StateId;
                objCompany.MSMENumber = obj.MSMENumber;
                objCompany.BankName = obj.BankName;
                objCompany.BankBranch = obj.BankBranch;
                objCompany.BankAccNumber = obj.BankAccNumber;
                objCompany.IFSCCode = obj.IFSCCode;
                objCompany.Warehouses = obj.Warehouses; 
                objCompany.DefaultWarehouseId = obj.DefaultWarehouseId;

                if (!String.IsNullOrEmpty(obj.Logo) && obj.Logo == "delete")
                {
                    objCompany.Logo = "";
                }
               else if (!String.IsNullOrEmpty(obj.Logo) && obj.Logo != "no")
                {


                    var file = new DataUrlHelper();

                    file.Parse(obj.Logo);

                    // get the file content-type "image/png"
                    var contentType = file.ContentType;
                    var fileName = obj.CompanyId + "_logo";
                    if (contentType.ToLower().Contains("png"))
                    {
                        fileName = fileName + ".png";
                    }
                    else
                    {
                        fileName = fileName + ".jpeg";
                    }

                    var fullFilePath = mapPath + "/comp/" + fileName;

                    // get the file stream binary data
                    var fileStream = file.FileStream;
                    byte[] fileData = new byte[fileStream.Length];
                    fileStream.Read(fileData, 0, fileData.Length);


                    if (!File.Exists(fileName))
                    {
                        File.WriteAllBytes(fullFilePath, fileData);
                    }

                    objCompany.Logo = fileName;

                }
                if (!String.IsNullOrEmpty(obj.Signature) && obj.Signature == "delete")
                {
                    objCompany.Signature = "";
                }
                else if (!String.IsNullOrEmpty(obj.Signature) && obj.Signature != "no")
                {


                    var file = new DataUrlHelper();

                    file.Parse(obj.Signature);

                    // get the file content-type "image/png"
                    var contentType = file.ContentType;
                    var fileName = obj.CompanyId + "_signature";
                    if (contentType.ToLower().Contains("png"))
                    {
                        fileName = fileName + ".png";
                    }
                    else
                    {
                        fileName = fileName + ".jpeg";
                    }

                    var fullFilePath = mapPath + "/comp/" + fileName;

                    // get the file stream binary data
                    var fileStream = file.FileStream;
                    byte[] fileData = new byte[fileStream.Length];
                    fileStream.Read(fileData, 0, fileData.Length);


                    if (!File.Exists(fileName))
                    {
                        File.WriteAllBytes(fullFilePath, fileData);
                    }

                    objCompany.Signature = fileName;

                }
                if (!String.IsNullOrEmpty(obj.QrCode) && obj.QrCode == "delete")
                {
                    objCompany.QrCode = "";
                }
                else if (!String.IsNullOrEmpty(obj.QrCode) && obj.QrCode != "no")
                {


                    var file = new DataUrlHelper();

                    file.Parse(obj.QrCode);

                    // get the file content-type "image/png"
                    var contentType = file.ContentType;
                    var fileName = obj.CompanyId + "_QrCode";
                    if (contentType.ToLower().Contains("png"))
                    {
                        fileName = fileName + ".png";
                    }
                    else
                    {
                        fileName = fileName + ".jpeg";
                    }

                    var fullFilePath = mapPath + "/comp/" + fileName;

                    // get the file stream binary data
                    var fileStream = file.FileStream;
                    byte[] fileData = new byte[fileStream.Length];
                    fileStream.Read(fileData, 0, fileData.Length);


                    if (!File.Exists(fileName))
                    {
                        File.WriteAllBytes(fullFilePath, fileData);
                    }

                    objCompany.QrCode = fileName;

                }

                msg.Code = ApiMessageCodes.SUCCESS;

                var packageService = new RentacPackageService();
                var package = packageService.ClientPackageSel(objCompany.RbnClientId);
                if (package != null)
                {
                    var getAll = Company.GetAll(new LoggedInUser().RbnClientId, "");
                    if (package.Companies == getAll.Count())
                    {
                        if (obj.CompanyId == 0)
                        {
                            //  throw new Exception("Maximum number of companies have been reached");
                            msg.Message = "Maximum number of companies have been reached";
                            msg.Code = ApiMessageCodes.ERROR;
                            return Request.CreateResponse(HttpStatusCode.OK, msg);
                        }
                    }
                }
                objCompany.Save();

                return Request.CreateResponse(HttpStatusCode.OK, msg);
            }
            catch (UDFException ex)
            {
                msg.Message = ex.Message;
                msg.Code = ApiMessageCodes.ERROR;
                return Request.CreateResponse(HttpStatusCode.OK, msg);

            }
            catch (Exception ex)
            {
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
                logger.Error(ex, ex.Message);
                //msg.Code = ex.ErrorCode.ToString();
                return Request.CreateResponse(HttpStatusCode.OK, msg);

            }
        }

        [HttpGet]
        public HttpResponseMessage GetAll()
        {

            var user = new LoggedInUser();
            var userDto = (new User()).GetById(user.UserId, user.FinYearId);

            var allCompanies = Company.GetAll(new LoggedInUser().RbnClientId);

            var companies = allCompanies.Where(o => o.ParentCompanyId == 0).ToList();

            if (userDto.RoleId != 1)
            {
                var _userCompanies = userDto.Companies.Split(',');
                companies = companies.Where(o => _userCompanies.Contains(o.CompanyId.ToString())).ToList();
            }


            return Request.CreateResponse(HttpStatusCode.OK, companies);
        }

        [HttpGet]
        public HttpResponseMessage Branches(int companyId)
        {
            var allCompanies = Company.GetAll(new LoggedInUser().RbnClientId, "");
            var companies = allCompanies.Where(o => o.ParentCompanyId == companyId).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, companies);
        }

        [HttpGet]
        public async Task<ApiMessage> GetTaxPayerDetails(string gstNo)
        {

            var gstService = new GSTService();
            var message = new ApiMessage();
            try
            {
                message.Data = await gstService.GetTaxPayerDetails(gstNo);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (Exception ex)
            {
                message.Message = ex.Message;
                message.Code = ApiMessageCodes.ERROR;
            }
            return message;
        }
        [HttpPost]
        public IHttpActionResult UpdateGSTDetails([FromBody] CompanyDTO obj)
        {
            Company objCompany = new Company();
            var message = new ApiMessage();
            try
            {
                if (obj == null)
                {
                    message.Code = ApiMessageCodes.ERROR;
                    message.Message = "Invalid input";
                    return Ok(message);
                }
                var user = new LoggedInUser();
                obj.CompanyId = user.DefaultCompanyId;
                var result = objCompany.UpdateGSTDetails(obj);
                if (result)
                {
                    message.Code = ApiMessageCodes.SUCCESS;
                }
                else
                {
                    message.Code = ApiMessageCodes.COULD_NOT_SUCCEED;

                }
                return Ok(message);
            }
            catch (Exception ex)
            {
                message.Message = ex.Message;
                message.Code = ApiMessageCodes.ERROR;
            }

            return Ok(message);

        }
        [HttpPost]
        public IHttpActionResult UpdateEInvoicing([FromBody] CompanyDTO obj)
        {
            Company objCompany = new Company();
            var message = new ApiMessage();
            try
            {
                if (obj == null)
                {
                    message.Code = ApiMessageCodes.ERROR;
                    message.Message = "Invalid input";
                    return Ok(message);
                }
                var user = new LoggedInUser();
                obj.CompanyId = user.DefaultCompanyId;
                obj.EInvoiceEnabledOn = DateTime.Now;
                obj.EInvoiceEnabledBy = user.UserId;
                var result = objCompany.UpdateEInvoiceEnabled(obj);
                if (result)
                {
                    message.Code = ApiMessageCodes.SUCCESS;
                }
                else
                {
                    message.Code = ApiMessageCodes.COULD_NOT_SUCCEED;

                }
                return Ok(message);
            }
            catch (Exception ex)
            {
                message.Message = ex.Message;
                message.Code = ApiMessageCodes.ERROR;
            }

            return Ok(message);

        }
        [HttpPost]
        public HttpResponseMessage ActivateDeActivate([FromBody] CompanyDTO obj)
        {
            Company objCompany = new Company(obj.CompanyId);
            objCompany.DeActivate(obj.IsActive);
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        [HttpPost]
        public HttpResponseMessage GetDetails([FromBody] CompanyDTO obj)
        {
            Company objCompany = new Company(obj.CompanyId);
            var dt = objCompany.GetDetails();
            if (dt != null)
            {
                if (dt.IRPToken != null)
                {
                    if (dt.IRPTokenExpiry < DateTime.Now)
                    {
                        dt.IRPTokenExpired = true;
                    }

                }
                if (!String.IsNullOrEmpty(dt.Logo))
                {
                    dt.Logo = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + "/docs/comp/" + dt.Logo;

                }
                if (!String.IsNullOrEmpty(dt.Signature))
                {
                    dt.Signature = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + "/docs/comp/" + dt.Signature;

                }
                if (!String.IsNullOrEmpty(dt.QrCode))
                {
                    dt.QrCode = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + "/docs/comp/" + dt.QrCode;

                }
                dt.IRPPassword = null;
                dt.IRPToken = null;
                dt.EwayPassword = null;
                if (!String.IsNullOrEmpty(dt.EwayUserName))
                {
                    dt.EwayBillEnabled = true;
                }
                //user information saved but not correct
                if (dt.EwayLastAuthenticatedOn.Year <= 1)
                {
                    dt.EwayConnected = false;
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, dt);
        }

        /// <summary>
        ///Actiates and de-activates an account group
        /// </summary>
        /// <param name="dto">AccountGroupDTO</param>
        /// <returns>true/false</returns>
        [HttpPost]
        public HttpResponseMessage SearchCompany([FromBody] CompanyDTO dto)
        {
            Company leder = new Company();
            //   leder.StoreId = 1;
            List<CompanyDTO> allLedgers = Company.GetAll(new LoggedInUser().RbnClientId, dto.Name);
            SearchResult<CompanyDTO> objResult = new SearchResult<CompanyDTO> { total_count = allLedgers.Count, incomplete_results = false, items = allLedgers };
            return Request.CreateResponse(HttpStatusCode.OK, objResult);
        }


    }
}
