using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class AddressDTO
    {
        public int AddressId { get; set; }
        public short AddressTypeId { get; set; }
        public int AddressHolderId { get; set; }
        public short RoleId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Email { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string Web { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public int StateId { get; set; }
        public string FullAddress
        {
            get
            {
                return (Address1 + " " + Address2 + " City: " + City + " State: " + State +
                    " Zipcode: " + ZipCode + " Phone:" + Phone1 + "," + Phone2 + " Email: " + Email
                    );
            }
        }
        //public string FullHTMLAddress
        //{
        //    get
        //    {
        //        return (Address1 + "<br/>" + Address2 + " City: " + City + " State: " + State +
        //            " Zipcode: " + ZipCode + "<br/>Phone:" + Phone1 + "," + Phone2 + "<br/>Email: " + Email
        //            );
        //    }
        //}
    }
}
