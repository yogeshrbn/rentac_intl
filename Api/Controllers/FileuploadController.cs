using BAL.DTO;
using BAL.Objects;
using BAL.Services;
using FarmaAPI.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.util;
using System.Web;
using System.Web.Http;

namespace FarmaAPI.Controllers
{
    //[Authorize]
    public class FileuploadController : BaseApiController
    {
        AzureStorageService storageService = new AzureStorageService();
        public FileuploadController() { }

        [HttpPost]
        public async Task<IHttpActionResult> upload()
        {
            var res = new ApiMessage();

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/temp/");
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
                if (provider.FileData == null || provider.FileData.Count == 0)
                {
                    res.Code = ApiMessageCodes.ERROR;
                    res.Message = "No file found in request payload.";
                    return Ok(res);
                }

                foreach (MultipartFileData fileData in provider.FileData)
                {
                    var headerFileName = fileData?.Headers?.ContentDisposition?.FileName;
                    string fileName = string.IsNullOrWhiteSpace(headerFileName) ? "upload.bin" : headerFileName.Trim('"');
                    string localFileName = fileData.LocalFileName;

                    var extension = GetSafeExtension(fileName, fileData?.Headers?.ContentType?.MediaType);
                    string newFileName = Guid.NewGuid().ToString() + extension;
                    string permanentPath = Path.Combine(root, newFileName);

                    if (File.Exists(permanentPath))
                    {
                        File.Delete(permanentPath);
                    }
                    File.Move(localFileName, permanentPath);
                    res.Data = newFileName;
                    res.Code = ApiMessageCodes.SUCCESS;

                    Console.WriteLine($"Uploaded file: {fileName} saved at: {localFileName}");
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                res.Code = ApiMessageCodes.ERROR;
                res.Message = "File upload failed.";
                res.Description = ex.Message;
                return Ok(res);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> downLoad([FromBody] StorageDocumentDto dto)
        {
            // Check if the request contains multipart/form-data

            var res = new ApiMessage();


            try
            {
                if(dto == null)
                {
                    throw new Exception("Input payload is null or empty");
                }

                var data = await storageService.DownloadFileAsync(dto.FinYearId, dto.CompanyId + "/" + dto.DocType, dto.FileName);
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(data)
                };
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = dto.FileName
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                var response = ResponseMessage(result);
                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string GetSafeExtension(string originalFileName, string mimeType)
        {
            var ext = Path.GetExtension(originalFileName);
            if (!string.IsNullOrWhiteSpace(ext))
            {
                return ext.StartsWith(".") ? ext : "." + ext;
            }

            if (!string.IsNullOrWhiteSpace(mimeType))
            {
                try
                {
                    var e = Utils.GetExtension(mimeType);
                    if (!string.IsNullOrWhiteSpace(e))
                    {
                        return e.StartsWith(".") ? e : "." + e;
                    }
                }
                catch
                {
                    // fallback below
                }
            }

            return ".bin";
        }
    }
}
