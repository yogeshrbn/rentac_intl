using BAL.DAL;
using BAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Objects
{
    public class Warehouse
    {


        public async Task<int> CreateWareHouse(WarehouseDTO dto)
        {
            var dal = new WarehouseDAL();
            return await dal.CreateWarehouse(dto);
        }
        public async Task<WarehouseDTO> UpdateWareHouse(WarehouseDTO dto)
        {
            var dal = new WarehouseDAL();
            return await dal.UpdateWarehouse(dto);
        }

        public async Task<IEnumerable<WarehouseDTO>> GetAll(WarehouseDTO dto)
        {
            var dal = new WarehouseDAL();
            return await dal.GetAll(dto);
        }

        public async Task<WarehouseDTO> UpdateStatus(WarehouseDTO dto)
        {
            var dal = new WarehouseDAL();
            return await dal.UpdateStatus(dto);
        }

        public async Task<WarehouseDTO> GetById(WarehouseDTO dto)
        {
            var dal = new WarehouseDAL();
            return await dal.GetById(dto);
        }
    }
}
