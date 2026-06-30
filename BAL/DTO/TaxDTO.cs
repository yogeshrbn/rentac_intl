using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class TaxDTO
    {
        public int ApplicableTaxId { get; set; }
        public BAL.Enums.TaxItem ItemId { get; set; }
        public int ItemValue { get; set; }
        public string Name { get; set; }
        public double Rate { get; set; }
        public double Amount { get; set; }
        public int TaxId { get; set; }
        public int ModifiedBy { get; set; }
        public double DefaultRate { get; set; }

        public double TaxAmount
        {
            get;
            set;
        }
    }
}
