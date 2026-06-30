using BAL.Data.Contracts;
using BAL.DTO;
using BAL.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services
{
    public class ProductTaxClassificationService : IProductTaxClassificationService
    {
        IProductTaxClassificationContract _contract;
        public ProductTaxClassificationService(IProductTaxClassificationContract contract)
        {
            _contract = contract;
        }
        public async Task<int> DeleteAsync(long taxClassificationId)
        {
            return await _contract.DeleteAsync(taxClassificationId);
        }

        public async Task<List<ProductTaxClassificationDto>> GetByProductAsync(int companyId, int productId)
        {
            return await _contract.GetByProductAsync(companyId, productId);
        }
        public async Task<ProductTaxClassificationDto> GetByTransactionAsync(int companyId, int productId, string transactionType)
        {
            return await _contract.GetByTransactionAsync(companyId, productId, transactionType);
        }
        public async Task<long> InsertAsync(SaveProductTaxClassificationDto dto)
        {
            return await _contract.InsertAsync(dto);
        }
        public async Task<int> UpdateAsync(long taxClassificationId, int taxCategoryId)
        {
            return await _contract.UpdateAsync(taxClassificationId, taxCategoryId);
        }
        public async Task UpsertAsync(SaveProductTaxClassificationDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
