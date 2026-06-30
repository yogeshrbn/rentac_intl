using BAL.DAL;
using BAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services
{
    public class ZoneService
    {
        public async Task<bool> Save(ZonesDTO dto)
        {
            var dal = new ZoneDAL();
            return await dal.Save(dto);
        }

        public async Task<IEnumerable<ZonesDTO>> ZonesList(ZonesDTO dto)
        {
            var dal = new ZoneDAL();
            return await dal.ZonesList(dto);
        }
        public async Task<ZonesDTO> ZoneById(ZonesDTO dto)
        {
            var dal = new ZoneDAL();
            return await dal.ZoneById(dto);
        }
        public async Task<int> DeleteZone(ZonesDTO dto)
        {
            var dal = new ZoneDAL();
            return await dal.DeleteZone(dto);
        }
    }
}
