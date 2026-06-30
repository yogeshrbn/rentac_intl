using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.DTO;
using BAL.Objects;
namespace FarmaAPI.Controllers
{
    [Authorize]
    public class ProductCategoryController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Save([FromBody] ProductCategoryDTO obj)
        {
            ProductCategory objCategory = new ProductCategory()
            {
                Name = obj.Name
                ,
                MinMargin = obj.MinMargin
                ,
                StoreId = obj.StoreId
                ,
                Status = obj.Status
                ,
                CategoryId = obj.CategoryId
            };
            objCategory.Save();
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        [HttpPost]
        public HttpResponseMessage GetAll([FromBody] ProductCategoryDTO obj)
        {
            ProductCategory objCategory = new ProductCategory();
            objCategory.GetAll(obj.StoreId);
            return Request.CreateResponse(HttpStatusCode.OK, objCategory.GetAll(obj.StoreId));
        }

        [HttpPost]
        public HttpResponseMessage GetDetail([FromBody] ProductCategoryDTO obj)
        {
            ProductCategory objCategory = new ProductCategory(obj.CategoryId);
            return Request.CreateResponse(HttpStatusCode.OK, objCategory);
        }
        [HttpPost]
        public HttpResponseMessage ChangeStatus([FromBody] ProductCategoryDTO obj)
        {
            ProductCategory objCategory = new ProductCategory(obj.CategoryId);
            objCategory.Status = obj.Status;
            objCategory.ChangeStatus();
            return Request.CreateResponse(HttpStatusCode.OK, objCategory);
        }
    }
}
