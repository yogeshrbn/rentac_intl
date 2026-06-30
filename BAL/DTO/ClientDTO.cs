using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
   public class ClientDTO : MasterDTO
    {
         int _clientId = 0;
        public int ClientId { get { return _clientId; } set { _clientId = value; } }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Contact { get; set; }
        public string ZipCode { get; set; }
        public string Web { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public bool IsActive { get; set; }
        public int AccountGroup { get; set; }

        public string TIN { get; set; }
        public string TAN { get; set; }
        /// <summary>
        /// used to filter based on screen
        /// </summary>
        public string Page { get; set; }
        public string Code { get; set; }


        public ClientDTO(int companyId)
        {
            _clientId = companyId;
        }

        public ClientDTO() { }
    }
}
