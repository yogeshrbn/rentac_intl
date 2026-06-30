using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class ProductCategoryDTO : StoreDTO
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public float MinMargin { get; set; }
        public ProductCategoryStatus Status { get; set; }
    }

    public enum ProductCategoryStatus
    {
        Active = 1,
        InActive = 2
    }
}
