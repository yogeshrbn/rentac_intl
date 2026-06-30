using BAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Data.Contracts
{
    public interface IProductTaxClassificationContract
    {
        Task<long> InsertAsync(SaveProductTaxClassificationDto dto);
        Task UpsertAsync(SaveProductTaxClassificationDto dto);
        Task<int> UpdateAsync(long taxClassificationId, int taxCategoryId);
        Task<int> DeleteAsync(long taxClassificationId);
        Task<List<ProductTaxClassificationDto>> GetByProductAsync(int companyId, int productId);
        Task<ProductTaxClassificationDto> GetByTransactionAsync(int companyId, int productId, string transactionType);
    }
}
