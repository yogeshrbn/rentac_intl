using BAL.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services.Contracts
{
    public interface IProductionService
    {
        Task<ProductionDto> GetByIdAsync(long productionId);
        Task<ProductionDto> GetByGuidAsync(string guid);
        Task<PagedResultDto<ProductionDto>> ListAsync(ProductionQueryDto query);
        Task<ProductionDto> CreateAsync(CreateProductionDto createDto);
        Task<ProductionDto> UpdateAsync(long productionId, UpdateProductionDto updateDto);
        Task<bool> DeleteAsync(long productionId);
        Task<IEnumerable<OperationDto>> GetOperation(int companyId);


    }
}
