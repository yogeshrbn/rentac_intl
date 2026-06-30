using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class Template
    {
        public int TemplateId { get; set; }
        public string Name { get; set; }
        public string TemplateGroup { get; set; }
        public string FileName { get; set; }
        public string SampleData { get; set; }
        public string Properties { get; set; }
        public bool IsDefault { get; set; }
        public string PreviewImage { get; set; }

        public byte Orientation { get; set; }
        public string Style { get; set; }
        public bool ApplyPrintConfig { get; set; } = false;

    }
}
