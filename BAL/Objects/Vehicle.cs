using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.Enums;
using BAL.DAL;
namespace BAL.Objects
{
    public class Vehicle : VehicleDTO
    {
        public bool Add()
        {
            VehicleDAL dal = new VehicleDAL();
            return dal.Add(this);
        }
        public List<VehicleDTO> GetAll(int companyId)
        {
            VehicleDAL dal = new VehicleDAL();
            return dal.GetAll(companyId);
        }
        public Vehicle() { }
        public Vehicle(int vechileId)
        {
            if (vechileId > 0)
            {
                VehicleDAL dal = new VehicleDAL();
                VehicleDTO dto = dal.GetById(vechileId);
                this.Name = dto.Name;
                this.RegNumber = dto.RegNumber;
                this.ChachisNumber = dto.ChachisNumber;
                this.EngineNumber = dto.EngineNumber;
                this.VehicleId = vechileId;
            }
        }
    }
}
