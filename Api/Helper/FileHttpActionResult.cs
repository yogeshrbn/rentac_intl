using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace FarmaAPI.Helper
{
    public class FileHttpActionResult : IHttpActionResult
    {
        private readonly byte[] _fileData;
        private readonly string _fileName;
        private readonly string _contentType;

        public FileHttpActionResult(byte[] fileData, string fileName, string contentType)
        {
            _fileData = fileData;
            _fileName = fileName;
            _contentType = contentType;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(_fileData)
            };

            response.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = _fileName
                };

            response.Content.Headers.ContentType =
                new MediaTypeHeaderValue(_contentType);

            return Task.FromResult(response);
        }
    }
}