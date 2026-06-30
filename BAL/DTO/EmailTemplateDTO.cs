using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class EmailTemplateDTO
    {
        public int TemplateId { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public int CompanyId { get; set; }
        public int RbnClientId { get; set; }
    }
}
