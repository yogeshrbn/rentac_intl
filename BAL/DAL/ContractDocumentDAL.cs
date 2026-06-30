using BAL.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BAL.DAL
{
    public class ContractDocumentDAL
    {
        const string INS_CONTRACT_DOCUMENT = "p_ContractDocument_ins";
        const string SEL_BY_CONTRACT = "p_ContractDocuments_selByContract";
        const string SEL_BY_JOB_CARDS = "p_ContractDocuments_selByJobCards";

        public async Task<IEnumerable<ContractDocumentDto>> ListByContractAsync(int companyId, int contractId)
        {
            var sql = new SQL();
            sql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            sql.AddParameter("@ContractId", DbType.Int32, ParameterDirection.Input, 0, contractId);
            return await sql.QueryAsync<ContractDocumentDto>(SEL_BY_CONTRACT);
        }

        public async Task<IEnumerable<ContractDocumentDto>> ListByJobCardsAsync(int companyId, IReadOnlyList<int> jobCardIds)
        {
            if (jobCardIds == null || jobCardIds.Count == 0)
                return Array.Empty<ContractDocumentDto>();

            var csv = string.Join(",", jobCardIds.Distinct().Where(id => id > 0));
            if (string.IsNullOrEmpty(csv))
                return Array.Empty<ContractDocumentDto>();

            var sql = new SQL();
            sql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            sql.AddParameter("@JobCardIds", DbType.String, ParameterDirection.Input, -1, csv);
            return await sql.QueryAsync<ContractDocumentDto>(SEL_BY_JOB_CARDS);
        }

        public async Task<int> InsertAsync(ContractDocumentDto dto)
        {
            var sql = new SQL();
            sql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
            sql.AddParameter("@ContractId", DbType.Int32, ParameterDirection.Input, 0, dto.ContractId);
            object jobCardParam = (dto.JobCardId.HasValue && dto.JobCardId.Value > 0) ? (object)dto.JobCardId.Value : DBNull.Value;
            sql.AddParameter("@JobCardId", DbType.Int32, ParameterDirection.Input, 0, jobCardParam);
            sql.AddParameter("@DocumentType", DbType.String, ParameterDirection.Input, 50, dto.DocumentType ?? (object)DBNull.Value);
            sql.AddParameter("@StoragePath", DbType.String, ParameterDirection.Input, 500, dto.StoragePath ?? (object)DBNull.Value);
            sql.AddParameter("@OriginalFileName", DbType.String, ParameterDirection.Input, 255, dto.OriginalFileName ?? (object)DBNull.Value);
            sql.AddParameter("@ContentType", DbType.String, ParameterDirection.Input, 100, dto.ContentType ?? (object)DBNull.Value);
            sql.AddParameter("@CreatedBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
            sql.AddParameter("@CreatedOn", DbType.DateTime2, ParameterDirection.Input, 0, dto.CreatedOn);

            var id = await sql.QueryFirstAsync<int>(INS_CONTRACT_DOCUMENT);
            return id;
        }
    }
}
