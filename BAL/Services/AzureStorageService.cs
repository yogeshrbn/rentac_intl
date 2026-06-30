using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BAL.Enums;
using BAL.Objects;
using NLog;
using static System.Net.WebRequestMethods;
 
namespace BAL.Services
{
    public class AzureStorageService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string storageContainer = ConfigurationManager.AppSettings["AzStorageContainer"];
        private static string storage = ConfigurationManager.AppSettings["storage"];

        public static string ContainerBaseUrl { get; set; }
        public static string ConnectionString { get; set; }

        public async Task<bool> UploadFileAsync(int finyearId, string containerName, string docName, string filePath)
        {
            //if (finyearId <= 0)
            //{
            //    throw new Exception("Invalid financial year Id for loggedInuser");
            //}            
            docName = finyearId + "/" + containerName + "/" + docName;

            return await UploadFileAsync(docName, filePath);
        }
        public async Task<bool> UploadFileAsync(int finyearId, string containerName, string docName, byte[] buffer)
        {
            if (finyearId <= 0)
            {
                throw new Exception("Invalid financial year Id for loggedInuser");
            }
            docName = finyearId + "/" + containerName + "/" + docName;


            return await UploadFileAsync(docName, buffer);
        }
      
        public async Task<byte[]> DownloadFileAsync(int finyearId, string containerName, string docName)
        {
            //if (finyearId <= 0)
            //{
            //    throw new Exception("Invalid financial year Id for loggedInuser");
            //}
            // docName = finyearId + "/" + Utils.GetEnumValue(docType) + "/" + docName;
            docName = finyearId + "/" + containerName + "/" + docName;
            return await DownloadFileAsync(docName);
        }
        public async Task<bool> DownloadFileAsync(int finyearId, ContainerDocType docType, string docName, string filePath)
        {
            //if (finyearId <= 0)
            //{
            //    throw new Exception("Invalid financial year Id for loggedInuser");
            //}
            docName = finyearId + "/" + Utils.GetEnumValue(docType) + "/" + docName;
            return await DownloadFileAsync(docName, filePath);
        }
        public async Task<bool> UploadFileAsync(string blobName, string filePath)
        {
            try
            {
                if (storage.ToLower() == "local")
                {
                    return true;
                }

                string connectionString = ConnectionString;
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);


                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(storageContainer);

                // Create the container if it doesn't exist
                await containerClient.CreateIfNotExistsAsync();

                BlobClient blobClient = containerClient.GetBlobClient(blobName);
                string mimeType = System.Web.MimeMapping.GetMimeMapping(filePath);
                // Upload the file
                using (FileStream fileStream = System.IO.File.OpenRead(filePath))
                {

                    await blobClient.UploadAsync(fileStream, overwrite: true);
                    blobClient.SetHttpHeaders(new BlobHttpHeaders { ContentType = mimeType });

                    //  await blobClient.UploadAsync(fileStream,new BlobHttpHeaders { ContentType = "application/pdf" }, overwrite: true,);

                }
                var x = blobClient.Uri.ToString();
                return true;
            }
            catch (Exception ex)
            {
                logger.Error($"Error in uploading file to azure storage account:  {ex.Message},containerName: {storageContainer}, blobName: {blobName}");
                return false;
            }
        }

        public async Task<bool> UploadFileAsync(string blobName, byte[] buffer)
        {
            try
            {
                string connectionString = ConnectionString;
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(storageContainer);

                // Create the container if it doesn't exist
                await containerClient.CreateIfNotExistsAsync();

                BlobClient blobClient = containerClient.GetBlobClient(blobName);
                string mimeType = System.Web.MimeMapping.GetMimeMapping(blobName);
                // Upload the file
                using (var stream = new MemoryStream(buffer))
                {
                    // var blobInfo = await blobClient.UploadAsync(stream, overwrite: true);
                    //blobClient.SetHttpHeaders(new BlobHttpHeaders { ContentType = "application/pdf" });
                    await blobClient.UploadAsync(stream, overwrite: true);
                    blobClient.SetHttpHeaders(new BlobHttpHeaders { ContentType = mimeType });
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.Error($"Error in uploading file to azure storage account:  {ex.Message},containerName: {storageContainer}, blobName: {blobName}");
                return false;
            }
        }
        private async Task<bool> DownloadFileAsync(string blobName, string filePath)
        {
            try
            {


                string connectionString = ConnectionString;
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(storageContainer);

                // Create the container if it doesn't exist
                await containerClient.CreateIfNotExistsAsync();

                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                // download the filed
                using (FileStream fileStream = System.IO.File.OpenWrite(filePath))
                {
                    await blobClient.DownloadToAsync(fileStream);
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.Error($"Error in downloading file from azure storage account:  {ex.Message},containerName: {storageContainer}, blobName: {blobName}");
                return false;
            }
        }

        private async Task<byte[]> DownloadFileAsync(string blobName)
        {
            try
            {
                string connectionString = ConnectionString;
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(storageContainer);

                // Create the container if it doesn't exist
                await containerClient.CreateIfNotExistsAsync();

                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                // download the filed

                using (var stream = new MemoryStream())
                {
                    await blobClient.DownloadToAsync(stream);
                    return stream.ToArray();
                }


            }
            catch (Exception ex)
            {
                logger.Error($"Error in downloading file from azure storage account:  {ex.Message},containerName: {storageContainer}, blobName: {blobName}");
                return null;
            }
        }
    }
}
