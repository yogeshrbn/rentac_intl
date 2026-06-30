using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using Dapper;

namespace BAL.Common
{
    internal sealed class ReceiptPathRow
    {
        public int LedgerTransactionId { get; set; }
        public string ReceiptDocumentPath { get; set; }
    }

    /// <summary>
    /// Stages quick-receipt images under temp/, then moves to docs/receipt/ on successful save.
    /// </summary>
    public static class QuickReceiptDocumentHelper
    {
        public const string StagingRelativePrefix = "temp/quick-receipt/";
        public const string PermanentRelativePrefix = "docs/receipt/";

        public static bool IsAllowedImageContentType(string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType)) return false;
            return contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsValidStagingRelativePath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return false;
            var p = relativePath.Replace('\\', '/').TrimStart('/');
            if (p.Contains("..")) return false;
            return p.StartsWith(StagingRelativePrefix, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsValidPermanentRelativePath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return false;
            var p = relativePath.Replace('\\', '/').TrimStart('/');
            if (p.Contains("..")) return false;
            return p.StartsWith(PermanentRelativePrefix, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Physical path for a content-relative path (temp/... or docs/...).</summary>
        public static string MapToPhysical(string relativePath)
        {
            var ctx = HttpContext.Current;
            if (ctx == null)
                throw new InvalidOperationException("Quick receipt file operations require an HTTP request context.");
            var normalized = relativePath.Replace('\\', '/').TrimStart('/');
            return ctx.Server.MapPath("/" + normalized);
        }

        /// <summary>Save upload to staging folder; returns web-relative path (temp/quick-receipt/...).</summary>
        public static string SaveStagingFile(HttpPostedFile file, int companyId)
        {
            if (file == null || file.ContentLength == 0)
                throw new ArgumentException("No file uploaded.");
            if (!IsAllowedImageContentType(file.ContentType))
                throw new ArgumentException("Only image files are allowed.");

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(ext) || ext.Length > 8)
                ext = ".bin";
            ext = ext.ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
            if (!allowed.Contains(ext))
                throw new ArgumentException("Unsupported image type.");

            var safeName = "QR-" + companyId + "-" + Guid.NewGuid().ToString("N") + ext;
            var relativeDir = StagingRelativePrefix + companyId + "/";
            var relativePath = relativeDir + safeName;

            var physicalDir = MapToPhysical(relativeDir);
            Directory.CreateDirectory(physicalDir);
            var physicalFile = Path.Combine(physicalDir, safeName);
            file.SaveAs(physicalFile);

            return relativePath.Replace('\\', '/');
        }

        /// <summary>Move staged file to permanent location; returns web-relative path (docs/receipt/...).</summary>
        public static string MoveStagingToPermanent(string stagingRelativePath, int ledgerTransactionId, int companyId)
        {
            if (!IsValidStagingRelativePath(stagingRelativePath))
                throw new ArgumentException("Invalid staging path.");

            var src = MapToPhysical(stagingRelativePath);
            if (!File.Exists(src))
                throw new FileNotFoundException("Staged receipt file was not found.");

            var ext = Path.GetExtension(src);
            var destName = "QR-LT-" + ledgerTransactionId + "-" + Guid.NewGuid().ToString("N") + ext;
            var relativeDest = PermanentRelativePrefix + companyId + "/" + destName;
            var physicalDestDir = MapToPhysical(PermanentRelativePrefix + companyId + "/");
            Directory.CreateDirectory(physicalDestDir);
            var physicalDest = Path.Combine(physicalDestDir, destName);

            if (File.Exists(physicalDest))
                File.Delete(physicalDest);

            try
            {
                File.Move(src, physicalDest);
            }
            catch (IOException)
            {
                File.Copy(src, physicalDest, true);
                File.Delete(src);
            }

            return relativeDest.Replace('\\', '/');
        }

        public static void TryDeletePhysicalForWebPath(string webRelativePath)
        {
            if (string.IsNullOrWhiteSpace(webRelativePath)) return;
            if (!IsValidPermanentRelativePath(webRelativePath) && !IsValidStagingRelativePath(webRelativePath))
                return;
            try
            {
                var p = MapToPhysical(webRelativePath);
                if (File.Exists(p))
                    File.Delete(p);
            }
            catch
            {
                /* ignore cleanup failures */
            }
        }

        public static Dictionary<int, string> GetReceiptPathsByIds(IEnumerable<int> ledgerTransactionIds, int companyId)
        {
            var ids = ledgerTransactionIds?.Distinct().Where(id => id > 0).ToList() ?? new List<int>();
            if (ids.Count == 0)
                return new Dictionary<int, string>();

            var cs = ConfigurationManager.ConnectionStrings["sqlCon"]?.ConnectionString;
            if (string.IsNullOrEmpty(cs))
                return new Dictionary<int, string>();

            using (var con = new SqlConnection(cs))
            {
                con.Open();
                var rows = con.Query<ReceiptPathRow>(
                    @"SELECT LedgerTransactionId, ReceiptDocumentPath
                      FROM dbo.LedgerTransactions
                      WHERE companyId = @companyId
                        AND LedgerTransactionId IN @ids",
                    new { companyId, ids });
                return rows.Where(r => r != null).ToDictionary(r => r.LedgerTransactionId, r => r.ReceiptDocumentPath ?? "");
            }
        }

        public static string GetReceiptPathForId(int ledgerTransactionId, int companyId)
        {
            var d = GetReceiptPathsByIds(new[] { ledgerTransactionId }, companyId);
            return d.TryGetValue(ledgerTransactionId, out var p) ? p : null;
        }
    }
}
