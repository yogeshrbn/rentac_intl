using BAL.DTO;
using BAL.Exceptions;
using BAL.Objects;
using BAL.Services;
using FarmaAPI.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
namespace FarmaAPI.Controllers
{
    [Authorize]
    public class ProductController : BaseApiController
    {
        ILogger _logger = LogManager.GetCurrentClassLogger();
        string tempRoot = HttpContext.Current.Server.MapPath("~/temp/");
        AzureStorageService storageService = new AzureStorageService();
        [HttpPost]
        public async Task<HttpResponseMessage> Save()
        {
            var msg = new ApiMessage();
            string dto = System.Web.HttpContext.Current.Request["dto"];
            System.Web.HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            JObject jsonObject = new JObject();
            Product objProduct = new Product();
            try
            {
                var user = new LoggedInUser();
                jsonObject = JObject.Parse(dto);

                objProduct.ProductId = Convert.ToInt32(jsonObject.GetValue("ProductId"));
                objProduct.Name = Convert.ToString(jsonObject.GetValue("Name"));
                objProduct.Category = Convert.ToInt32(jsonObject.GetValue("Category"));
                objProduct.Code = Convert.ToString(jsonObject.GetValue("Code"));
                objProduct.SACCode = Convert.ToString(jsonObject.GetValue("SACCode"));

                objProduct.CompanyId = new LoggedInUser().DefaultCompanyId;
                if (jsonObject.GetValue("SortOrder") != null)
                {
                    objProduct.SortOrder = Convert.ToInt16(jsonObject.GetValue("SortOrder"));
                }
                //objProduct.CST = Convert.ToBoolean(jsonObject.GetValue("CST"));
                //objProduct.CSTRate = Convert.ToInt32(jsonObject.GetValue("CSTRate"));
                //objProduct.ExiseRate = Convert.ToInt32(jsonObject.GetValue("ExiseRate"));
                //objProduct.LocalTax = Convert.ToInt32(jsonObject.GetValue("LocalTax"));
                objProduct.MRP = Convert.ToDouble(jsonObject.GetValue("MRP"));

                //objProduct.Packing = obj.Packing;
                //objProduct.PackingQty = obj.PackingQty;
                objProduct.ProductType = Convert.ToInt32(jsonObject.GetValue("ProductType"));
                objProduct.PurchaseRate = Convert.ToInt32(jsonObject.GetValue("PurchaseRate"));
                objProduct.Salt = Convert.ToInt32(jsonObject.GetValue("Salt"));
                objProduct.StoreId = Convert.ToInt32(jsonObject.GetValue("StoreId"));
                objProduct.Unit = Convert.ToString(jsonObject.GetValue("Unit"));
                objProduct.VatRate = Convert.ToDouble(jsonObject.GetValue("VatRate"));
                objProduct.Description = Convert.ToString(jsonObject.GetValue("Description"));
                objProduct.UOM = Convert.ToInt16(jsonObject.GetValue("UOM"));
                var uom2Token = jsonObject.GetValue("UOM2");
                if (uom2Token != null && uom2Token.Type != JTokenType.Null
                    && short.TryParse(uom2Token.ToString(), out short uom2Val) && uom2Val > 0)
                {
                    objProduct.UOM2 = uom2Val;
                }
                else
                {
                    objProduct.UOM2 = null;
                }
                if (jsonObject["ApplyUnit2Rate"] != null && jsonObject["ApplyUnit2Rate"].Type != JTokenType.Null)
                {
                    objProduct.ApplyUnit2Rate = Convert.ToBoolean(jsonObject.GetValue("ApplyUnit2Rate"));
                }
                if (jsonObject.GetValue("Size") != null)
                {
                    objProduct.Size = Convert.ToString(jsonObject.GetValue("Size"));
                }
                if (jsonObject.GetValue("Weight") != null)
                {
                    objProduct.Weight = Convert.ToDouble(jsonObject.GetValue("Weight"));
                }
                if (jsonObject.GetValue("UnitSizeRate") != null)
                {
                    objProduct.UnitSizeRate = Convert.ToDouble(jsonObject.GetValue("UnitSizeRate"));
                }
                if (jsonObject.GetValue("WeightRate") != null)
                {
                    objProduct.WeightRate = Convert.ToDouble(jsonObject.GetValue("WeightRate"));
                }
                objProduct.RbnClientId = new LoggedInUser().RbnClientId;
                objProduct.OpeningBalance = Convert.ToDouble(jsonObject.GetValue("OpeningBalance"));

                objProduct.CreatedBy = user.UserId;
                objProduct.RbnClientId = user.RbnClientId;
                objProduct.CreationDate = DateTime.Now;
                objProduct.FinYearId = user.FinYearId;
                objProduct.TaxCategoryId = Convert.ToInt16(jsonObject.GetValue("TaxCategoryId"));
                objProduct.SaleAccount = Convert.ToInt32(jsonObject.GetValue("SaleAccount"));
                objProduct.SalePrice = Convert.ToDouble(jsonObject.GetValue("SalePrice"));
                objProduct.CostPrice = Convert.ToDouble(jsonObject.GetValue("CostPrice"));
                objProduct.PurchaseAccount = Convert.ToInt32(jsonObject.GetValue("PurchaseAccount"));
                var purchaseUnitToken = jsonObject.GetValue("PurchaseUnitId");
                if (purchaseUnitToken != null && purchaseUnitToken.Type != JTokenType.Null
                    && short.TryParse(purchaseUnitToken.ToString(), out short purchaseUnitId) && purchaseUnitId > 0)
                {
                    objProduct.PurchaseUnitId = purchaseUnitId;
                }
                else
                {
                    objProduct.PurchaseUnitId = null;
                }
                objProduct.HSNCode = Convert.ToString(jsonObject.GetValue("HSNCode"));
                objProduct.RentRate = Convert.ToDouble(jsonObject.GetValue("RentRate"));
                objProduct.BrekageRate = Convert.ToDouble(jsonObject.GetValue("BrekageRate"));
                objProduct.LossRate = Convert.ToDouble(jsonObject.GetValue("LossRate"));
                if (jsonObject.GetValue("ItemGroupId") != null)
                    objProduct.ItemGroupId = Convert.ToInt32(jsonObject.GetValue("ItemGroupId"));

                objProduct.Image1 = Convert.ToString(jsonObject.GetValue("Image1"));

                var sizes = jsonObject["ProductSize"];
                bool ret = await objProduct.Save();
                msg.Code = ApiMessageCodes.SUCCESS;

                if (ret && !String.IsNullOrEmpty(objProduct.Image1))
                {
                    var fileToSave = tempRoot + objProduct.Image1;
                    if (File.Exists(fileToSave))
                    {
                        var uploaded = await storageService.UploadFileAsync(0, user.DefaultCompanyId + "/docs", objProduct.Image1, fileToSave);
                        if (uploaded)
                        {
                            File.Delete(fileToSave);
                        }
                    }
                }


                return Request.CreateResponse(HttpStatusCode.OK, msg);
                //return Request.CreateResponse(HttpStatusCode.OK, objProduct);
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
                msg.Message = "unknown error occurred, please try again in sometime";
                //msg.Code = ex.ErrorCode.ToString();
                return Request.CreateResponse(HttpStatusCode.OK, msg);

            }


        }
        public async Task<HttpResponseMessage> Save_v1([FromBody] Product objProduct)
        {
            var msg = new ApiMessage();
            //string dto = System.Web.HttpContext.Current.Request["dto"];
            //  System.Web.HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            JObject jsonObject = new JObject();

            try
            {
                var user = new LoggedInUser();
                //jsonObject = JObject.Parse(dto);

                //objProduct.ProductId = Convert.ToInt32(jsonObject.GetValue("ProductId"));
                //objProduct.Name = Convert.ToString(jsonObject.GetValue("Name"));
                //objProduct.Category = Convert.ToInt32(jsonObject.GetValue("Category"));
                //objProduct.Code = Convert.ToString(jsonObject.GetValue("Code"));
                //objProduct.SACCode = Convert.ToString(jsonObject.GetValue("SACCode"));

                //objProduct.CompanyId = new LoggedInUser().DefaultCompanyId;
                //if (jsonObject.GetValue("SortOrder") != null)
                //{
                //    objProduct.SortOrder = Convert.ToInt16(jsonObject.GetValue("SortOrder"));
                //}
                //objProduct.CST = Convert.ToBoolean(jsonObject.GetValue("CST"));
                //objProduct.CSTRate = Convert.ToInt32(jsonObject.GetValue("CSTRate"));
                //objProduct.ExiseRate = Convert.ToInt32(jsonObject.GetValue("ExiseRate"));
                //objProduct.LocalTax = Convert.ToInt32(jsonObject.GetValue("LocalTax"));
                //  objProduct.MRP = Convert.ToDouble(jsonObject.GetValue("MRP"));

                //objProduct.Packing = obj.Packing;
                //objProduct.PackingQty = obj.PackingQty;
                //objProduct.ProductType = Convert.ToInt32(jsonObject.GetValue("ProductType"));
                //objProduct.PurchaseRate = Convert.ToInt32(jsonObject.GetValue("PurchaseRate"));
                //objProduct.Salt = Convert.ToInt32(jsonObject.GetValue("Salt"));
                //objProduct.StoreId = Convert.ToInt32(jsonObject.GetValue("StoreId"));
                //objProduct.Unit = Convert.ToString(jsonObject.GetValue("Unit"));
                //objProduct.VatRate = Convert.ToDouble(jsonObject.GetValue("VatRate"));
                //objProduct.Description = Convert.ToString(jsonObject.GetValue("Description"));
                //objProduct.UOM = Convert.ToInt16(jsonObject.GetValue("UOM"));
                //if (jsonObject.GetValue("Size") != null)
                //{
                //    objProduct.Size = Convert.ToString(jsonObject.GetValue("Size"));
                //}
                //if (jsonObject.GetValue("Weight") != null)
                //{
                //    objProduct.Weight = Convert.ToDouble(jsonObject.GetValue("Weight"));
                //}
                //if (jsonObject.GetValue("UnitSizeRate") != null)
                //{
                //    objProduct.UnitSizeRate = Convert.ToDouble(jsonObject.GetValue("UnitSizeRate"));
                //}
                //if (jsonObject.GetValue("WeightRate") != null)
                //{
                //    objProduct.WeightRate = Convert.ToDouble(jsonObject.GetValue("WeightRate"));
                //}
                //objProduct.RbnClientId = new LoggedInUser().RbnClientId;
                //objProduct.OpeningBalance = Convert.ToDouble(jsonObject.GetValue("OpeningBalance"));

                objProduct.CreatedBy = user.UserId;
                objProduct.RbnClientId = user.RbnClientId;
                objProduct.CreationDate = DateTime.Now;
                objProduct.FinYearId = user.FinYearId;
                objProduct.CompanyId = user.DefaultCompanyId;
                //objProduct.TaxCategoryId = Convert.ToInt16(jsonObject.GetValue("TaxCategoryId"));
                //objProduct.SaleAccount = Convert.ToInt32(jsonObject.GetValue("SaleAccount"));
                //objProduct.SalePrice = Convert.ToDouble(jsonObject.GetValue("SalePrice"));
                //objProduct.CostPrice = Convert.ToDouble(jsonObject.GetValue("CostPrice"));
                //objProduct.PurchaseAccount = Convert.ToInt32(jsonObject.GetValue("PurchaseAccount"));
                //objProduct.HSNCode = Convert.ToString(jsonObject.GetValue("HSNCode"));
                //objProduct.RentRate = Convert.ToDouble(jsonObject.GetValue("RentRate"));
                //objProduct.BrekageRate = Convert.ToDouble(jsonObject.GetValue("BrekageRate"));
                //objProduct.LossRate = Convert.ToDouble(jsonObject.GetValue("LossRate"));
                //if (jsonObject.GetValue("ItemGroupId") != null)
                //    objProduct.ItemGroupId = Convert.ToInt32(jsonObject.GetValue("ItemGroupId"));

                //objProduct.Image1 = Convert.ToString(jsonObject.GetValue("Image1"));

                var sizes = jsonObject["ProductSize"];
                bool ret = await objProduct.Save();
                msg.Code = ApiMessageCodes.SUCCESS;

                if (ret && !String.IsNullOrEmpty(objProduct.Image1))
                {
                    var fileToSave = tempRoot + objProduct.Image1;
                    if (File.Exists(fileToSave))
                    {
                        var uploaded = await storageService.UploadFileAsync(0, user.DefaultCompanyId + "/docs", objProduct.Image1, fileToSave);
                        if (uploaded)
                        {
                            File.Delete(fileToSave);
                        }
                    }
                }


                return Request.CreateResponse(HttpStatusCode.OK, msg);
                //return Request.CreateResponse(HttpStatusCode.OK, objProduct);
            }
            catch (UDFException ex)
            {
                msg.Message = ex.Message;
                msg.Code = ApiMessageCodes.ERROR;
                _logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.OK, msg);

            }
            catch (Exception ex)
            {
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = "unknown error occurred, please try again in sometime:" + ex.Message;
                //msg.Code = ex.ErrorCode.ToString();
                _logger.Error(ex);
                return Request.CreateResponse(HttpStatusCode.OK, msg);

            }


        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetAll([FromBody] ProductDTO obj)
        {
            Product objProduct = new Product();
            List<ProductDTO> objProducts = await objProduct.GetAll(new LoggedInUser().DefaultCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, objProducts);
        }

        [HttpPost]
        public async Task<ApiMessage> DeleteProduct([FromBody] ProductDTO obj)
        {
            var msg = new ApiMessage();
            try
            {
                Product objProduct = new Product();
                msg.Data = await objProduct.DeleteProductId(obj.ProductId, new LoggedInUser().DefaultCompanyId);
                msg.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                msg.Code = ApiMessageCodes.SUCCESS;
                msg.Message = ex.Message;
            }

            return msg;
        }

        [HttpPost]
        public HttpResponseMessage GetDetail([FromBody] ProductDTO obj)
        {
            ProductDTO objProduct = new Product(obj.ProductId);
            return Request.CreateResponse(HttpStatusCode.OK, objProduct);
        }

        [HttpPost]
        public HttpResponseMessage GetAllUOM()
        {
            UOM objUOM = new UOM();
            return Request.CreateResponse(HttpStatusCode.OK, objUOM.GetAll());
        }
        [HttpPost]
        public HttpResponseMessage GetUOMSize([FromBody] UOMDTO obj)
        {
            UOM objUOM = new UOM();
            objUOM.UOMId = obj.UOMId;
            return Request.CreateResponse(HttpStatusCode.OK, objUOM.GetSize());
        }
        [HttpPost]
        public HttpResponseMessage AddRate([FromBody] RentRateDTO obj)
        {
            Product objProduct = new Product(0);
            obj.EffectiveDate = Utils.FormatDate(obj.EffectiveDate).ToShortDateString();
            objProduct.AddRate(obj);
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }
        [HttpPost]
        public HttpResponseMessage GetRates([FromBody] RentRateDTO obj)
        {
            Product objProduct = new Product(0);
            return Request.CreateResponse(HttpStatusCode.OK, objProduct.GetRates(obj.ProductId));
        }
        [HttpPost]
        public HttpResponseMessage ActivateRate([FromBody] RentRateDTO obj)
        {
            Product objProduct = new Product(0);
            return Request.CreateResponse(HttpStatusCode.OK, objProduct.ActivateRate(obj.Active, obj.RentRateId));
        }
        [HttpPost]
        public HttpResponseMessage ADDSize([FromBody] UOMSizeDTO obj)
        {
            Product objProduct = new Product(0);
            return Request.CreateResponse(HttpStatusCode.OK, objProduct.AddUOMSize(obj));
        }
        [HttpPost]
        public HttpResponseMessage GetAllSize([FromBody] UOMSizeDTO obj)
        {
            Product objProduct = new Product(0);
            return Request.CreateResponse(HttpStatusCode.OK, objProduct.GetALLSize());
        }
        [HttpPost]
        public HttpResponseMessage ActivateSize([FromBody] UOMSizeDTO obj)
        {
            Product objProduct = new Product(0);
            return Request.CreateResponse(HttpStatusCode.OK, objProduct.ActviateSize(obj.Active, obj.UOMSizeId));
        }


        [HttpPost]
        public HttpResponseMessage Search([FromBody] ProductDTO dto)
        {
            Product objProduct = new Product();
            List<ProductDTO> products = objProduct.Search(dto.Name, new LoggedInUser().DefaultCompanyId);

            SearchResult<ProductDTO> objResult = new SearchResult<ProductDTO> { total_count = products.Count, incomplete_results = false, items = products };

            //result += Request.CreateResponse(HttpStatusCode.OK, objProduct.Search(name)) + "}";

            return Request.CreateResponse(HttpStatusCode.OK, objResult);
        }
        [HttpPost]
        public HttpResponseMessage GetProductSizeList([FromBody] ProductDTO dto)
        {
            Product objProduct = new Product();
            List<ProductSizeDTO> products = objProduct.ProductSizeList(dto.ProductId, new LoggedInUser().FinYearId);
            return Request.CreateResponse(HttpStatusCode.OK, products);
        }
        [HttpPost]
        public HttpResponseMessage GetProductSizeListByCompany([FromBody] ProductDTO dto)
        {
            Product objProduct = new Product();
            List<ProductSizeDTO> products = objProduct.ProductSizeListByCompany(new LoggedInUser().DefaultCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, products);
        }

        [HttpPost]
        public async Task<ApiMessage> SaveBom([FromBody] BOMDTO dto)
        {
            var message = new ApiMessage();
            try
            {
                Product objProduct = new Product();
                var user = new LoggedInUser();
                dto.CreatedBy = user.UserId;
                dto.CompanyId = user.DefaultCompanyId;
                dto.CreatedOn = DateTime.Now;
                dto.ModifiedOn = DateTime.Now;
                dto.ModifiedBy = user.UserId;
                dto.GuId = Guid.NewGuid().ToString();
                message.Data = await objProduct.SaveBOM(dto);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                throw ex;
            }
        }
        [HttpPost]
        public async Task<ApiMessage> BOMList()
        {
            var message = new ApiMessage();
            try
            {
                Product objProduct = new Product();
                var user = new LoggedInUser();
                message.Data = await objProduct.BOMList(user.DefaultCompanyId);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ApiMessage> BOMDetails([FromBody] BOMDTO dto)
        {
            var message = new ApiMessage();
            try
            {
                Product objProduct = new Product();
                var user = new LoggedInUser();
                message.Data = await objProduct.BOMById(dto.BomId, user.DefaultCompanyId);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                throw ex;
            }
        }
        [HttpPost]
        public async Task<ApiMessage> BOMByProductId([FromBody] BOMDTO dto)
        {
            var message = new ApiMessage();
            try
            {
                Product objProduct = new Product();
                var user = new LoggedInUser();
                message.Data = await objProduct.BOMByProductId(dto.ProductId, user.DefaultCompanyId);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                throw ex;
            }
        }
        [HttpPost]
        public async Task<ApiMessage> DeleteBom([FromBody] BOMDTO dto)
        {
            var message = new ApiMessage();
            try
            {
                Product objProduct = new Product();
                var user = new LoggedInUser();
                message.Data = await objProduct.DeleteBOM(dto.BomId, user.DefaultCompanyId);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ApiMessage> SaveItemGroup([FromBody] ItemGroupDTO dto)
        {
            var message = new ApiMessage();
            try
            {
                Product objProduct = new Product();
                var user = new LoggedInUser();
                dto.CompanyId = user.DefaultCompanyId;
                dto.CreatedBy = user.UserId;
                dto.CreatedOn = DateTime.Now;
                dto.ModifiedBy = user.UserId;
                dto.ModifiedOn = DateTime.Now;
                dto.GuId = Guid.NewGuid().ToString();

                message.Data = await objProduct.SaveGroup(dto);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ApiMessage> ListItemGroup([FromBody] ItemGroupDTO dto)
        {
            var message = new ApiMessage();
            try
            {
                dto = new ItemGroupDTO();
                Product objProduct = new Product();
                var user = new LoggedInUser();
                dto.CompanyId = user.DefaultCompanyId;
                dto.CreatedBy = user.UserId;
                dto.CreatedOn = DateTime.Now;
                dto.ModifiedBy = user.UserId;
                dto.ModifiedOn = DateTime.Now;
                dto.GuId = Guid.NewGuid().ToString();

                message.Data = await objProduct.ListItemGroup(dto);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ApiMessage> ItemGroupById([FromBody] ItemGroupDTO dto)
        {
            var message = new ApiMessage();
            try
            {
                Product objProduct = new Product();
                var user = new LoggedInUser();
                dto.CompanyId = user.DefaultCompanyId;
                dto.CreatedBy = user.UserId;
                dto.CreatedOn = DateTime.Now;
                dto.ModifiedBy = user.UserId;
                dto.ModifiedOn = DateTime.Now;
                dto.GuId = Guid.NewGuid().ToString();

                message.Data = await objProduct.ItemGroupById(dto);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                throw ex;
            }
        }
        [HttpPost]
        public async Task<ApiMessage> DeleteItemGroup([FromBody] ItemGroupDTO dto)
        {
            var message = new ApiMessage();
            try
            {
                Product objProduct = new Product();
                var user = new LoggedInUser();
                dto.CompanyId = user.DefaultCompanyId;
                dto.CreatedBy = user.UserId;
                dto.CreatedOn = DateTime.Now;
                dto.ModifiedBy = user.UserId;
                dto.ModifiedOn = DateTime.Now;
                dto.GuId = Guid.NewGuid().ToString();

                message.Data = await objProduct.DeleteItemGroup(dto);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                throw ex;
            }
        }
    }


}
