using BAL.DAL;
using BAL.DTO;
using BAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Contracts.Repository
{
    public interface IProductionRepository
    {
        Task<Production> GetByIdAsync(long productionId);
        Task<Production> GetByGuidAsync(string guid);
        Task<PagedResultDto<Production>> ListAsync(ProductionQueryDto query);
        Task<long> InsertAsync(Production production);
        Task<bool> UpdateAsync(Production production);
        Task<bool> DeleteAsync(long productionId);

        Task<IEnumerable<OperationDto>> GetOperation(int companyId);

    }
}
