using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DAL;
namespace BAL.Objects
{
    public class ProductCategory : ProductCategoryDTO, IDisposable
    {
        ProductCategoryDAL _objDAL;
        public ProductCategory()
        {
            _objDAL = new ProductCategoryDAL();
        }
        public ProductCategory(int _categoryId)
        {

            _objDAL = new ProductCategoryDAL();

            this.CategoryId = _categoryId;
            if (this.CategoryId > 0)
            {
                //getInfo
                GetInfo();
            }
        }

        void GetInfo()
        {
            ProductCategoryDTO _dtoObject = _objDAL.GetInfo(this.CategoryId);
            this.Name = _dtoObject.Name;
            this.MinMargin = _dtoObject.MinMargin;
            this.Status = _dtoObject.Status;
            this.StoreId = _dtoObject.StoreId;
        }

        public int Save()
        {
            return _objDAL.Save(this);
        }

        public int ChangeStatus()
        {

            return _objDAL.ChangeStatus(this.CategoryId, this.Status);
        }

        public List<ProductCategoryDTO> GetAll(int storeId)
        {
            return _objDAL.GetAll(storeId);
        }
        public void Dispose()
        {
            if (_objDAL != null)
            {

            }
        }
    }
}
