using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class CommDTO
    {
        public string Recipient { get; set; }
        public int RbnClientId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public DateTime SentDate { get; set; }
        public string Message { get; set; }
        public string Template { get; set; }

        public string For { get; set; }
        public int MessageTransactionId { get; set; }
    }
}
