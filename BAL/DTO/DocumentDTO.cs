using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
   public class DocumentDTO
    {
       public string FilePath { get; set; }
       public string Title { get; set; }
       public DateTime UploadedDate { get; set; }
       public int UploadedBy { get; set; }
       public short FileType { get; set; }

    }

    public class StorageDocumentDto
    {
        public string FileName { get; set; }
        public int FinYearId { get; set; }
        public int CompanyId { get; set; }
        public string DocType { get; set; }
    }
}
