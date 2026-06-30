using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DAL;
namespace BAL.Objects
{
    public class UOM : UOMDTO
    {

        public List<UOMDTO> GetAll()
        {
            UOMDAL dal = new UOMDAL();
            return dal.GetAll(this);
        }

        public List<UOMSizeDTO> GetSize()
        {
            UOMDAL dal = new UOMDAL();
            return dal.GetSize(this.UOMId);
        }

    }
}
