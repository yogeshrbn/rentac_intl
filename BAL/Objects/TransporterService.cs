using BAL.DAL;
using BAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Objects
{
    public class TransporterService
    {
        public bool Save(TransporterDTO dto)
        {
            var dal = new TransporterDAL();
            return dal.Save(dto);
        }
        public List<TransporterDTO> GetAll(TransporterDTO dto)
        {
            var dal = new TransporterDAL();
            return dal.GetAll(dto);
        }
    }
}
