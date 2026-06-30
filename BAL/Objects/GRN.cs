using BAL.Common;
using BAL.DAL;
using BAL.DTO;
using BAL.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
namespace BAL.Objects
{
    public class GRN : GRNDTO
    {
        public bool Add()
        {
            GRNDAL grnDal = new GRNDAL();
            return grnDal.Add(this);

        }
      
        public async Task<GRNDTO> GrnById(int grnId, int companyId)
        {
            GRNDAL grnDal = new GRNDAL();
            return await grnDal.GrnById(grnId, companyId);
        }

        public List<GRNDTO> GetItemsByGrnId(int grnId, int companyId)
        {
            GRNDAL grnDal = new GRNDAL();
            return grnDal.GetItemsByGrnId(grnId, companyId);
        }
        public DataSet GRNHeader(int grnId)
        {
            GRNDAL grnDal = new GRNDAL();
            return grnDal.GRNHeader(grnId);
        }
        public async Task<IEnumerable<GRNChageDTO>> GetOtherChages(int grnId, int companyId)
        {
            GRNDAL grnDal = new GRNDAL();
            return await grnDal.GetOtherChages(grnId, companyId);
        }
        public async Task<int> DeleteChallan(int companyId, int grnId, LoggedInUserInfo user)
        {
            GRNDAL grnDal = new GRNDAL();
            return await grnDal.DeleteChallan(companyId, grnId, user);

        }

        public bool UpdateEwayBIllNo(int grnId, int companyId, string ewayBillNo)
        {
            try
            {
                GRNDAL grnDal = new GRNDAL();
                return grnDal.UpdateEwayBIllNo(grnId, companyId, ewayBillNo);
            }

            catch (Exception ex)
            {
                throw new UDFException("Could not update ewaybill no in challan", ErrorCodes.ERROR_WHILE_UPDATE_EWAYBILL_IN_CHALLAN, ex);
            }
        }
        public async Task<int> InwardConfirm(GRNDTO dto)
        {
            GRNDAL grnDal = new GRNDAL();
            return await grnDal.InwardConfirm(dto);
        }

        public async Task<string> LastChallanNumber(int companyId,int finYearId, int ChallanType)
        {
            GRNDAL grnDal = new GRNDAL();
            return await grnDal.LastChallanNumber(companyId, finYearId, ChallanType);

        }

        public async Task<string> GetNextReceivingChallanNumberPreview(int companyId, int finYearId, int challanType)
        {
            GRNDAL grnDal = new GRNDAL();
            return await grnDal.GetNextReceivingChallanNumberPreview(companyId, finYearId, challanType);
        }
    }
}
