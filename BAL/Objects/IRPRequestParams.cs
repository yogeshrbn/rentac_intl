using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Objects
{
    public class IRPRequestParams
    {
        public string Username { get; set; }
        public string Password{ get; set; }
        public string Token { get; set; }
        public string GSTIN { get; set; }
        

    }

    public class IRPToken
    {
        public string Username { get; set; }
        public string AuthToken { get; set; }
        public string ClientId { get; set; }

        public DateTime TokenExpiry { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public int CompanyId { get; set; }
        public string Sek { get; set; }


    }
}
