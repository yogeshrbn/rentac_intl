using BAL.Contracts.Repository;
using BAL.DTO;
using BAL.Models;
using BAL.Services.Contracts;
using Microsoft.Extensions.Logging;
using NLog;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services
{


    public class ProductionService : IProductionService
    {
        private readonly IProductionRepository _repository;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ProductionService(IProductionRepository repository )
        {
            _repository = repository;
            
        }

        public async Task<ProductionDto> GetByIdAsync(long productionId)
        {
            try
            {
                var production = await _repository.GetByIdAsync(productionId);
                return MapToDto(production);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting production by ID: {ProductionId}", productionId);
                throw;
            }
        }

        public async Task<ProductionDto> GetByGuidAsync(string guid)
        {
            try
            {
                var production = await _repository.GetByGuidAsync(guid);
                return MapToDto(production);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting production by GUID: {Guid}", guid);
                throw;
            }
        }

        public async Task<PagedResultDto<ProductionDto>> ListAsync(ProductionQueryDto query)
        {
            try
            {
                var result = await _repository.ListAsync(query);
                return new PagedResultDto<ProductionDto>
                {
                    Data = result.Data.Select(MapToDto).Where(x => x != null).ToList(),
                    TotalRecords = result.TotalRecords,
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error listing productions with query: {@Query}", query);
                throw;
            }
        }

        public async Task<ProductionDto> CreateAsync(CreateProductionDto createDto)
        {
            try
            {
                ValidateProductionDates(createDto.PlannedStartDate, createDto.PlannedEndDate,
                                      createDto.ActualStartDate, createDto.ActualEndDate);

                var production = new Production
                {
                    ProductId = createDto.ProductId,
                    Quantity = createDto.Quantity,
                    StatusId = createDto.StatusId,
                    CompanyId = createDto.CompanyId,
                    ClientId = createDto.ClientId,
                    SaleOrderNo = createDto.SaleOrderNo,
                    Description = createDto.Description,
                    PlannedStartDate = createDto.PlannedStartDate,
                    ActualStartDate = createDto.ActualStartDate,
                    ActualEndDate = createDto.ActualEndDate,
                    PlannedEndDate = createDto.PlannedEndDate,
                    CreatedBy = createDto.CreatedBy,
                    CreatedOn = DateTime.UtcNow,
                    BOM = createDto.BOM.Select(o=> new ProductionBOMDto().InjectFrom(o)).Cast<ProductionBOMDto>().ToList(),
                    Operations = createDto.Operations.Select(o => new ProductionOperationDto().InjectFrom(o)).Cast<ProductionOperationDto>().ToList()

                };

                await _repository.InsertAsync(production);

                return MapToDto(production);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating production: {@CreateDto}", createDto);
                throw;
            }
        }

        public async Task<ProductionDto> UpdateAsync(long productionId, UpdateProductionDto updateDto)
        {
            try
            {
                var existing = await _repository.GetByIdAsync(productionId);
                if (existing == null)
                    return null;

                ValidateProductionDates(updateDto.PlannedStartDate, updateDto.PlannedEndDate,
                                      updateDto.ActualStartDate, updateDto.ActualEndDate);

                existing.ProductId = updateDto.ProductId;
                existing.Quantity = updateDto.Quantity;
                existing.StatusId = updateDto.StatusId;
                existing.CompanyId = updateDto.CompanyId;
                existing.ClientId = updateDto.ClientId;
                existing.SaleOrderNo = updateDto.SaleOrderNo;
                existing.Description = updateDto.Description;
                existing.PlannedStartDate = updateDto.PlannedStartDate;
                existing.ActualStartDate = updateDto.ActualStartDate;
                existing.ActualEndDate = updateDto.ActualEndDate;
                existing.PlannedEndDate = updateDto.PlannedEndDate;
                existing.ModifiedBy = updateDto.ModifiedBy;
                existing.ModifiedOn = DateTime.UtcNow;

                var updated = await _repository.UpdateAsync(existing);
                return updated ? MapToDto(existing) : null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating production {ProductionId}: {@UpdateDto}", productionId, updateDto);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(long productionId)
        {
            try
            {
                return await _repository.DeleteAsync(productionId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting production: {ProductionId}", productionId);
                throw;
            }
        }

        private static ProductionDto MapToDto(Production production)
        {
            if (production == null) return null;

            return new ProductionDto
            {
                ProductionId = production.ProductionId,
                ProductId = production.ProductId,
                Quantity = production.Quantity,
                StatusId = production.StatusId,
                CompanyId = production.CompanyId,
                ClientId = production.ClientId,
                SaleOrderNo = production.SaleOrderNo,
                Description = production.Description,
                PlannedStartDate = production.PlannedStartDate,
                ActualStartDate = production.ActualStartDate,
                ActualEndDate = production.ActualEndDate,
                PlannedEndDate = production.PlannedEndDate,
                CreatedBy = production.CreatedBy,
                CreatedOn = production.CreatedOn,
                ModifiedBy = production.ModifiedBy,
                ModifiedOn = production.ModifiedOn,
                GuId = production.GuId
            };
        }

        private static void ValidateProductionDates(DateTime plannedStart, DateTime plannedEnd,
                                                  DateTime actualStart, DateTime actualEnd)
        {
            if (plannedStart > plannedEnd)
                throw new ArgumentException("Planned start date cannot be after planned end date");

            if (actualStart > actualEnd)
                throw new ArgumentException("Actual start date cannot be after actual end date");
        }

        public async Task<IEnumerable<OperationDto>> GetOperation(  int companyId)
        {
            return await this._repository.GetOperation( companyId);
        }
    }
}