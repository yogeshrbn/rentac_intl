using BAL.DAL;
using BAL.DTO;
using BAL.Exceptions;
using NLog;
using Razorpay.Api.Errors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BAL.Objects
{
    public class Contract
    {
        ContractDAL dal = new ContractDAL();
        Logger _logger = LogManager.GetCurrentClassLogger();
        public async Task<bool> Save(ContractDTO dto)
        {
            return await dal.Save(dto);
        }

        public async Task<IEnumerable<ContractViewDto>> GetAll(ContractFilterDto filterDto)
        {
            return await dal.GetAll(filterDto);
        }
        public async Task<ContractViewDto> GetById(ContractFilterDto filter)
        {
            var cTask = await dal.GetById(filter);
            var contract = cTask.First();

            if (contract == null)
            {
                throw new UDFException("No Contract Found", Exceptions.ErrorCodes.CONTRACT_NOT_FOUND);
            }
            var details = await dal.GetContractDetails(filter);
            var conditions = await dal.GetContractConditions(filter);

            var cDal = new CompanyDAL();
            var cLedger = new LedgerDAL();

            contract.Company = cDal.GetDetails(contract.CompanyId);
            contract.Ledger = cLedger.GetDetails(contract.LedgerId);



            if (details != null)
                contract.Details = details.ToList();
            if (conditions != null)
                contract.Conditions = conditions.ToList();

            return contract;
        }

        public async Task<bool> UpdateContractStatus(ContractDTO cdto)
        {
            return await dal.UpdateContractStatus(cdto);
        }

        public async Task<IEnumerable<WorkOrderItemDTO>> ContractInventory(ContractFilterDto filter)
        {
            return await dal.ContractInventory(filter);
        }
        public async Task<bool> ExtendContract(ContractDTO cdto)
        {
            return await dal.ExtendContract(cdto);
        }
        public async Task<IEnumerable<BillingDTO>> GetContractBills(int companyId, int contractId)
        {
            var billdal = new BillingDAL();
            return await billdal.GetContractBills(companyId, contractId);
        }

        public async Task<ServiceResult<int>> CreateActivityAsync(ContractActivity activity)
        {
            try
            {
                var _repository = new ContractDAL();

                // Validate activity
                var validationResult = await ValidateActivityAsync(activity);
                if (!validationResult.Success)
                    return ServiceResult<int>.Failure(validationResult.Message);

                // Set audit fields
                var now = DateTime.UtcNow;
                activity.CreatedOn = now;
                activity.ModifiedOn = now;

                var activityId = await _repository.InsertActivityAsync(activity);

                _logger.Info("Contract activity created with ID: {ActivityId}", activityId);

                return ServiceResult<int>.SuccessResult(activityId, "Contract activity created successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating contract activity");
                return ServiceResult<int>.Failure("Error creating contract activity");
            }
        }

        public async Task<ServiceResult<bool>> UpdateActivityAsync(ContractActivity activity)
        {
            try
            {
                var _repository = new ContractDAL();

                // Validate activity
                var validationResult = await ValidateActivityAsync(activity);
                if (!validationResult.Success)
                    return ServiceResult<bool>.Failure(validationResult.Message);

                // Set audit fields
                var now = DateTime.UtcNow;
                activity.CreatedOn = now;
                activity.ModifiedOn = now;

                var activityId = await _repository.UpdateActivityAsync(activity);

                _logger.Info("Contract activity created with ID: {ActivityId}", activityId);

                return ServiceResult<bool>.SuccessResult(activityId, "Contract activity created successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating contract activity");
                return ServiceResult<bool>.Failure("Error creating contract activity");
            }
        }
        public async Task<ServiceResult<bool>> DeleteActivityAsync(ContractActivity activity)
        {
            try
            {
                var _repository = new ContractDAL();

                // Validate activity

                // Set audit fields
                var now = DateTime.UtcNow;
                activity.CreatedOn = now;
                activity.ModifiedOn = now;

                var activityId = await _repository.DeleteActivityAsync(activity.ActivityId, activity.CompanyId);

                _logger.Info("Contract activity deleted with ID: {ActivityId}", activityId);

                return ServiceResult<bool>.SuccessResult(activityId, "Contract activity created successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting contract activity");
                return ServiceResult<bool>.Failure("Error deleting contract activity");
            }
        }

        public async Task<ServiceResult<ContractActivity>> GetByIdAsync(ContractActivity activity)
        {
            try
            {
                var _repository = new ContractDAL();

                // Validate activity

                // Set audit fields
                var now = DateTime.UtcNow;
                activity.CreatedOn = now;
                activity.ModifiedOn = now;

                var activityId = await _repository.GetByIdAsync(activity.ActivityId, activity.CompanyId);

                _logger.Info("Contract activity deleted with ID: {ActivityId}", activityId);

                return ServiceResult<ContractActivity>.SuccessResult(activityId, "Contract activity created successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting contract activity");
                return ServiceResult<ContractActivity>.Failure("Error deleting contract activity");
            }
        }
        public async Task<ServiceResult<IEnumerable<ContractActivity>>> GetAllAsync(ContractActivity activity)
        {
            try
            {
                var _repository = new ContractDAL();

                // Validate activity

                // Set audit fields
                var now = DateTime.UtcNow;
                activity.CreatedOn = now;
                activity.ModifiedOn = now;

                IEnumerable<ContractActivity> activityId = await _repository.GetAllAsync(activity.ContractId, activity.CompanyId);

                _logger.Info("Contract activity deleted with ID: {ActivityId}", activityId);

                return ServiceResult<IEnumerable<ContractActivity>>.SuccessResult(activityId, "Contract activity created successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting contract activity");
                return ServiceResult<IEnumerable<ContractActivity>>.Failure("Error deleting contract activity");
            }
        }
        public async Task<ServiceResult<bool>> ValidateActivityAsync(ContractActivity activity)
        {
            try
            {
                var errors = new List<string>();

                // Required field validation
                if (activity.CompanyId <= 0)
                    errors.Add("Company ID is required");

                if (activity.ContractId <= 0)
                    errors.Add("Contract ID is required");

                if (activity.TeamId <= 0)
                    errors.Add("Team ID is required");

                if (string.IsNullOrWhiteSpace(activity.ActivityStatus))
                    errors.Add("Activity status is required");

                if (activity.CreatedBy <= 0)
                    errors.Add("Created by user ID is required");

                if (activity.ModifiedBy <= 0)
                    errors.Add("Modified by user ID is required");

                // Business rule validation
                if (activity.ActivityStatus.Length > 50)
                    errors.Add("Activity status cannot exceed 50 characters");

                if (activity.Employees?.Length > 4000) // Approximate nvarchar(max) limit for practical purposes
                    errors.Add("Employees data is too large");

                if (errors.Any())
                {
                    return ServiceResult<bool>.Failure(string.Join("; ", errors));
                }

                return ServiceResult<bool>.SuccessResult(true, "Validation successful");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error validating contract activity");
                return ServiceResult<bool>.Failure("Error validating contract activity");
            }
        }

        public async Task<IEnumerable<ContractActivityTrackerDTO>> ActivityTracker(FilterCriteria filter)
        {
            var _repository = new ContractDAL();
            return await _repository.ActivityTracker(filter);

        }
        public async Task<IEnumerable<EmployeeDPRDTO>> EmployeeDPR(FilterCriteria filter)
        {
            var _repository = new ContractDAL();
            return await _repository.EmployeeDPR(filter);

        }

        /// <summary>Inserts metadata for a contract-linked file (storage path points to blob).</summary>
        public async Task<int> InsertContractDocumentAsync(ContractDocumentDto dto)
        {
            var docDal = new ContractDocumentDAL();
            return await docDal.InsertAsync(dto);
        }

        /// <summary>Lists contract documents (uploaded files metadata) for a contract.</summary>
        public async Task<IEnumerable<ContractDocumentDto>> GetContractDocumentsAsync(ContractFilterDto filter)
        {
            var docDal = new ContractDocumentDAL();
            return await docDal.ListByContractAsync(filter.CompanyId, filter.ContractId);
        }

        public async Task<IEnumerable<WorkOrderItemDTO>> ContractDelChallanItems(int contractId, int companyId)
        {
            var workOrderDAL = new WorkorderDAL();
            return await workOrderDAL.ContractDelChallanItems(contractId, companyId);
        }
        public async Task<IEnumerable<WorkOrderItemDTO>> ContractReturnChallanItems(int contractId, int companyId)
        {
            var workOrderDAL = new WorkorderDAL();
            return await workOrderDAL.ContractReturnChallanItems(contractId, companyId);
        }
    }

}
