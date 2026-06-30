using BAL.DAL;
using BAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Objects
{
    public class JobCard
    {
        public async Task<bool> Save(JobCardDto dto)
        {
            var jobCard = new JobCardDAL();
            return await jobCard.Save(dto);
        }
        public async Task<IEnumerable<JobCardDto>> GetList(JobCardDto dto)
        {
            var jobCard = new JobCardDAL();
            return await jobCard.GetList(dto);
        }

        public async Task<IEnumerable<WorkOrderItemDTO>> JobCardChallanItems(int jobCardId, int companyId)
        {
            var workOrderDAL = new WorkorderDAL();
            return await workOrderDAL.JobCardChallanItems(jobCardId, companyId);
        }
      
        

        public async Task<IEnumerable<WorkOrderItemDTO>> JobCardReturnChallanItems(int jobCardId, int companyId)
        {
            var workOrderDAL = new WorkorderDAL();
            return await workOrderDAL.JobCardReturnChallanItems(jobCardId, companyId);
        }
     
        public async Task<int> UpdateStatus(JobCardDto dto)
        {
            var dal = new JobCardDAL();
            return await dal.UpdateStatus(dto);
        }
        public async Task<JobCardDto> GetById(JobCardDto dto)

        {
            var dal = new JobCardDAL();

            return await dal.GetById(dto);
        }
    }
}
