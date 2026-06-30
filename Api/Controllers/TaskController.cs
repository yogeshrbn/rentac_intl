using BAL.DTO;
using BAL.Services;
using FarmaAPI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace FarmaAPI.Controllers
{
    public class TaskController : BaseApiController
    {
        LoggingService logger = new LoggingService();
        TaskService taskService = new TaskService();

        [HttpPost]
        public async Task<ApiMessage> Save([FromBody] TaskDTO dto)
        {
            var msg = new ApiMessage();
            try
            {
                var user = new LoggedInUser();

                dto.CreatedBy = user.UserId;
                dto.CreatedOn = DateTime.Now;
                dto.CompanyId = user.DefaultCompanyId;
                dto.GuId = Guid.NewGuid().ToString();
                dto.ModifiedBy = user.UserId;

                dto.ModifiedOn = DateTime.Now;

                msg.Data = await taskService.Save(dto);
                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<ApiMessage> AssignTask(TaskDTO dto)
        {
            var msg = new ApiMessage();
            try
            {
                var user = new LoggedInUser();

                dto.AssignedBy = user.UserId;
                dto.AssignedOn = DateTime.Now;
                dto.CompanyId = user.DefaultCompanyId;


                msg.Data = await taskService.AssignTask(dto);
                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<ApiMessage> ListTasks(TaskFilterDto dto)
        {
            var msg = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                dto.UserId = user.UserId;
                dto.CompanyId = user.DefaultCompanyId;
                var data = await taskService.ListTasks(dto);
                foreach(var item in data) {
                    if (item.StatusId == 1)
                    {
                        item.StatusName = "Assigned";
                    }
                    else if (item.StatusId == 2) { item.StatusName = "Active"; }
                    else if (item.StatusId == 3) { item.StatusName = "In-Progress"; }
                    else if (item.StatusId == 4) { item.StatusName = "Deleted"; }
                    else if (item.StatusId == 5) { item.StatusName = "Completed"; }
                }
                msg.Data =data;
                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<ApiMessage> TaskById(TaskDTO dto)
        {
            var msg = new ApiMessage();
            try
            {
                var user = new LoggedInUser();

                dto.CompanyId = user.DefaultCompanyId;
                msg.Data = await taskService.TaskById(dto);
                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<ApiMessage> UpdateStatus(TaskDTO dto)
        {
            var msg = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                dto.ModifiedBy = user.UserId;
                dto.ModifiedOn= DateTime.Now;
                dto.CompanyId = user.DefaultCompanyId;
                msg.Data = await taskService.UpdateStatus(dto);
                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        }
}
