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

        public async Task<IEnumerable<TaxCategoryDTO>> GetAllTaxCategories()
        {
            TaxDAL dal = new TaxDAL();

            return await dal.GetAllTaxCategories();
        }

        public async Task<TaxCategoryDTO> GetTaxCategoryById(int taxCategoryId)
        {
            TaxDAL dal = new TaxDAL();
            return await dal.GetTaxCategoryById(taxCategoryId);
        }

        public async Task<int> SaveTaxCategory(TaxCategoryDTO dto)
        {
            TaxDAL dal = new TaxDAL();
            return await dal.SaveTaxCategory(dto);
        }

        public async Task DeleteTaxCategory(int taxCategoryId)
        {
            TaxDAL dal = new TaxDAL();
            await dal.DeleteTaxCategory(taxCategoryId);
        }

        public async Task<IEnumerable<TaxMasterDTO>> GetAllTaxMasters(int companyId)
        {
            TaxDAL dal = new TaxDAL();
            return await dal.GetAllTaxMasters(companyId);
        }

        public async Task<TaxMasterDTO> GetTaxMasterById(int id, int companyId)
        {
            TaxDAL dal = new TaxDAL();
            return await dal.GetTaxMasterById(id, companyId);
        }

        public async Task<int> SaveTaxMaster(TaxMasterDTO dto)
        {
            TaxDAL dal = new TaxDAL();
            return await dal.SaveTaxMaster(dto);
        }
        public async Task<IEnumerable<TaxCategoryMappingDTO>> GetTaxesByCategoryId(int taxCategoryId)
        {
            TaxDAL dal = new TaxDAL();
            return await dal.GetTaxesByCategoryId(taxCategoryId);
        }
        //public abstract void SaveTax(int id = 0);

    }
}
