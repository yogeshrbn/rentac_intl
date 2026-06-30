using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DAL;
namespace BAL.Objects
{
    public class Salt : SaltDTO
    {
        public Salt()
        {
        }
        public Salt(int _saltId)
        {
            this.SaltId = _saltId;
            GetInfo();
        }
        void GetInfo()
        {
            SaltDAL objDal = new SaltDAL();
            SaltDTO dto = objDal.Get(this.SaltId);

            this.Name = dto.Name;
            this.Dosage = dto.Dosage;
            this.DrugInstructions = dto.DrugInstructions;
            this.Indications = dto.Indications;
            this.Narcotic = dto.Narcotic;
            this.SCH_H = dto.SCH_H;
            this.SCH_H1 = dto.SCH_H1;
            this.SideEffects = dto.SideEffects;
            this.Status = dto.Status;
            this.StoreId = dto.StoreId;
            this.CreatedBy = dto.CreatedBy;
            this.Precautions = dto.Precautions;
            this.Note = dto.Note;


        }
        public void Save()
        {
            SaltDAL objDal = new SaltDAL();
            objDal.Save(this);
        }

        public void ChangeStatus()
        {
            SaltDAL objDal = new SaltDAL();
            switch (Status)
            {
                case 1:
                    objDal.ActivateDeActivate(SaltStatus.Active, SaltId);
                    break;
                case 2:
                    objDal.ActivateDeActivate(SaltStatus.InActive, SaltId);

                    break;
            }


        }

        public List<SaltDTO> GetAll(int storeId)
        {
            SaltDAL objDal = new SaltDAL();
            return objDal.GetAll(storeId);
        }

    }
}
