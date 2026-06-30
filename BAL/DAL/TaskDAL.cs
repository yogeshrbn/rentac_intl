using BAL.DTO;
using BAL.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DAL
{
    public class TaskDAL
    {
        LoggingService logger = new LoggingService();
        public async Task<bool> Save(TaskDTO dto)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@task", DbType.String, ParameterDirection.Input, 0, dto.Task);

                sql.AddParameter("@deliveryDate", DbType.Date, ParameterDirection.Input, 0, dto.DeliveryDate);
                sql.AddParameter("@assignedTo", DbType.Int32, ParameterDirection.Input, 0, dto.AssignedTo);


                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);


                if (dto.TaskId == 0)
                {                  
                    sql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedOn);
                    sql.AddParameter("@createdBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
                    sql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, dto.GuId);

                    var result = await sql.ExecuteNonQueryAsync(ADD_TASK);
                    return result > 0;
                }
                else
                {
                    sql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
                    sql.AddParameter("@modifiedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedOn);
                    sql.AddParameter("@taskId", DbType.Int32, ParameterDirection.Input, 0, dto.TaskId);
                    var result = await sql.ExecuteNonQueryAsync(UPD_TASK);
                    return result > 0;
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<bool> AssignTask(TaskDTO dto)
        {
            var sql = new SQL();
            try
            {
            
                sql.AddParameter("@assingedTo", DbType.Int32, ParameterDirection.Input, 0, dto.AssignedTo);
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                sql.AddParameter("@assignedBy", DbType.Int32, ParameterDirection.Input, 0, dto.AssignedBy);
                sql.AddParameter("@assignedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.AssignedOn);

                sql.AddParameter("@taskId", DbType.Int32, ParameterDirection.Input, 0, dto.TaskId);
                var result = await sql.ExecuteNonQueryAsync(ASSIGN_TASK);
                return result > 0;


            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<TaskDTO>> ListTasks(TaskFilterDto dto)
        {
            var sql = new SQL();
            try
            {
                //sql.AddParameter("@from", DbType.DateTime, ParameterDirection.Input, 0, dto.From);
                //sql.AddParameter("@to", DbType.DateTime, ParameterDirection.Input, 0, dto.To);
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                sql.AddParameter("@status", DbType.Int16, ParameterDirection.Input, 0, dto.StatusId);
                sql.AddParameter("@userId", DbType.Int32, ParameterDirection.Input, 0, dto.UserId);
                return await sql.QueryAsync<TaskDTO>(TASK_LIST);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<TaskDTO> TaskById(TaskDTO dto)
        {
            var sql = new SQL();
            try
            {
           
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
              
                sql.AddParameter("@taskId", DbType.Int32, ParameterDirection.Input, 0, dto.TaskId);
                return await sql.QueryFirstAsync<TaskDTO>(TASK_BYID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<bool> UpdateStatus(TaskDTO dto)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@taskId", DbType.Int32, ParameterDirection.Input, 0, dto.TaskId);
                sql.AddParameter("@statusId", DbType.Int16, ParameterDirection.Input, 0, dto.StatusId);
               
                sql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);
                sql.AddParameter("@modifiedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);

                var result = await sql.ExecuteNonQueryAsync(UPDATE_STATUS);
                return result > 0;


            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        #region procedures
        const string ADD_TASK = "p_tasks_ins";
        const string UPD_TASK = "p_tasks_upd";
        const string ASSIGN_TASK = "p_tasks_assign";
        const string TASK_LIST = "p_tasks_list";
        const string UPDATE_STATUS = "p_tasks_updStatus";
        const string TASK_BYID = "p_tasks_byId";

        #endregion
    }
}
