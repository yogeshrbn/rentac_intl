using BAL.Data.Repository;
using BAL.DTO;
using FarmaAPI.Helper;
using NLog;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace FarmaAPI.Controllers
{
    [Authorize]
    public class ProductTaxClassificationController : BaseApiController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Returns the tax classification for a product and transaction type (e.g. Rental for issue challan).
        /// </summary>
        [HttpGet]
        public async Task<ApiMessage> ByTransaction(int productId, string transactionType = "Rental")
        {
            var msg = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                var repo = new ProductTaxClassificationRepository();
                var data = await repo.GetByTransactionAsync(user.DefaultCompanyId, productId, transactionType);
                msg.Data = data;
                msg.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
            }

            return msg;
        }
    }
}
