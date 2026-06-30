using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class SaltDTO
    {
        public int SaltId { get; set; }
        public string Name { get; set; }
        public string Indications { get; set; }
        public string Dosage { get; set; }
        public string SideEffects { get; set; }
        public string Precautions { get; set; }
        public string DrugInstructions { get; set; }
        public string Note { get; set; }

        public int Status { get; set; }
        public bool Narcotic { get; set; }
        public bool SCH_H { get; set; }
        public bool SCH_H1 { get; set; }
        public int StoreId { get; set; }
        public int CreatedBy { get; set; }


        public SaltDTO()
        {
        }
    }

    public enum SaltStatus
    {
        Active = 1,
        InActive = 2
    }
}
