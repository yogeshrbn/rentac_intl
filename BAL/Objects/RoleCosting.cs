using BAL.DAL;
using BAL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BAL.Objects
{
    public class RoleCosting : RoleCostingDTO
    {
        public async Task<IEnumerable<RoleCostingDTO>> List(int companyId)
        {
            var dal = new RoleCostingDAL();
            return await dal.List(companyId);
        }

        public async Task<RoleCostingDTO> ById(int roleCostingId, int companyId)
        {
            var dal = new RoleCostingDAL();
            return await dal.ById(roleCostingId, companyId);
        }

        public async Task<int> Save(RoleCostingDTO dto)
        {
            var dal = new RoleCostingDAL();
            return await dal.Save(dto);
        }

        public async Task<int> Delete(RoleCostingDTO dto)
        {
            var dal = new RoleCostingDAL();
            return await dal.Delete(dto);
        }
    }
}
