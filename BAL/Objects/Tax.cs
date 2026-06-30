using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.Interface;
using BAL.DAL;
namespace BAL.Objects
{
    public class Tax
    {
        public List<TaxDTO> Taxes { get; set; }
        public Tax()
        {

        }
        public List<TaxDTO> GetApplicableTaxes(Enums.TaxItem item, int itemValue)
        {
            TaxDAL dal = new TaxDAL();
            return dal.GetApplicableTaxes(item, itemValue);
        }

        public bool Save(List<TaxDTO> dto)
        {
            TaxDAL dal = new TaxDAL();
            return dal.Save(dto);
        }

        public List<TaxDTO> GetAllTaxes(int companyId)
        {
            TaxDAL dal = new TaxDAL();
            return dal.GetAllTaxes(companyId);
        }


        //public abstract void GetTaxes();
        //public abstract void SaveTax(int id = 0);

    }
}
