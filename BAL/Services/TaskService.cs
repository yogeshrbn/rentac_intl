using BAL.DAL;
using BAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services
{
    public class TaskService
    {
        public async Task<bool> Save(TaskDTO dto)
        {
            var dal = new TaskDAL();
            return await dal.Save(dto);
        }
        public async Task<bool> AssignTask(TaskDTO dto)
        {
            var dal = new TaskDAL();
            return await dal.AssignTask(dto);
        }
        public async Task<IEnumerable<TaskDTO>> ListTasks(TaskFilterDto dto)
        {
            var dal = new TaskDAL();
            return await dal.ListTasks(dto);
        }

        public async Task<TaskDTO> TaskById(TaskDTO dto)
        {
            var dal = new TaskDAL();
            return await dal.TaskById(dto);
        }
        public async Task<bool> UpdateStatus(TaskDTO dto)
        {
            var dal = new TaskDAL();
            return await dal.UpdateStatus(dto);
        }
    }
}
