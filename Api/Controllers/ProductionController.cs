using BAL.DTO;
using BAL.Models;
using BAL.Services.Contracts;
using FarmaAPI.Helper;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Windows.Interop;



namespace FarmaAPI.Controllers
{
    [Authorize]
    public class ProductionController : BaseApiController
    {


        private readonly IProductionService _productionService;
        //  private readonly ILogger<ProductionController> _logger;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ProductionController(IProductionService productionService)
        {
            _productionService = productionService;

        }

        [HttpGet]
        public async Task<IHttpActionResult> GetById(long id)
        {
            var msg = new ApiMessage();
            try
            {

                var production = await _productionService.GetByIdAsync(id);
                if (production == null)
                {
                    msg.Code = ApiMessageCodes.NOT_FOUND;
                    return Ok(msg);
                }

                return Ok(production);
            }
            catch (Exception ex)
            {

                _logger.Error(ex, "Error getting production by ID: {ProductionId}", id);
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
                return Ok(msg);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetByGuid(string guid)
        {
            var msg = new ApiMessage();
            try
            {
                var production = await _productionService.GetByGuidAsync(guid);
                if (production == null)
                {
                    msg.Code = ApiMessageCodes.NOT_FOUND;
                    return Ok(msg);
                }

                return Ok(production);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting production by GUID: {Guid}", guid);
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
                return Ok(msg);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> List([FromBody] ProductionQueryDto query)
        {
            var msg = new ApiMessage();
            try
            {
                msg.Data = await _productionService.ListAsync(query);
                msg.Code = ApiMessageCodes.SUCCESS;
                return Ok(msg);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error listing productions with query: {@Query}", query);
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
                return Ok(msg);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> Create([FromBody] CreateProductionDto createDto)
        {
            var msg = new ApiMessage();
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);


                var user = new LoggedInUser();
                createDto.CompanyId = user.DefaultCompanyId;
                createDto.CreatedBy = user.UserId;
                createDto.StatusId = 1;

                msg.Data = await _productionService.CreateAsync(createDto);
                msg.Code = ApiMessageCodes.SUCCESS;
                return Ok(msg);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating production: {@CreateDto}", createDto);
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
                return Ok(msg);
            }
        }

        [HttpPut]
        public async Task<IHttpActionResult> Update(long id, [FromBody] UpdateProductionDto updateDto)
        {
            var msg = new ApiMessage();
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var production = await _productionService.UpdateAsync(id, updateDto);
                if (production == null)
                {
                    msg.Code = ApiMessageCodes.NOT_FOUND;
                    return Ok(msg);
                }

                return Ok(production);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating production {ProductionId}: {@UpdateDto}", id, updateDto);
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
                return Ok(msg);
            }
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete(long id)
        {
            var msg = new ApiMessage();
            try
            {
                var deleted = await _productionService.DeleteAsync(id);
                if (!deleted)
                {
                    msg.Code = ApiMessageCodes.NOT_FOUND;
                    return Ok(msg);
                }
                msg.Code = ApiMessageCodes.SUCCESS;
                return Ok(msg);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting production: {ProductionId}", id);
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
                return Ok(msg);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> Operations()
        {
            var msg = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                var _data = await _productionService.GetOperation(user.DefaultCompanyId);

                msg.Data = _data;
                msg.Code = ApiMessageCodes.SUCCESS;
                return Ok(msg);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error fetching operations");
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
                return Ok(msg);
            }
        }
    }
}
