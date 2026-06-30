using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class VerificationLinkDTO
    {
        public string GuId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ValidTill { get; set; }
        public string Email { get; set; }
        public byte Used { get; set; }

    }
}
