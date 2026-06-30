using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using System.Data;
using BAL.Exceptions;
using System.Data.SqlClient;

namespace BAL.DAL
{
    internal class EmployeeDAL
    {

        internal bool Save(EmployeeDTO dto)
        {
            try
            {
                SQL sql = new SQL();
                sql.AddParameter("@Name", DbType.String, ParameterDirection.Input, 0, dto.Name);
                sql.AddParameter("@EmployeeCode", DbType.String, ParameterDirection.Input, 0, dto.EmployeeCode);
                sql.AddParameter("@Address", DbType.String, ParameterDirection.Input, 0, dto.Address);
                sql.AddParameter("@Aadhar", DbType.String, ParameterDirection.Input, 0, dto.Aadhar);
                sql.AddParameter("@Phone", DbType.String, ParameterDirection.Input, 0, dto.Phone);
                sql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                sql.AddParameter("@RoleId", DbType.Int32, ParameterDirection.Input, 0, dto.RoleId);
                if (dto.EmployeeId > 0)
                {
                    sql.AddParameter("@EmployeeId", DbType.Int32, ParameterDirection.Input, 0, dto.EmployeeId);
                    dto.EmployeeId = Convert.ToInt32(sql.ExecuteScalar(UPDATE));
                }
                else
                {
                    dto.EmployeeId = Convert.ToInt32(sql.ExecuteScalar(ADD));
                }
                return true;
            }
            catch (SqlException ex)
            {
                throw new UDFException(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        internal List<EmployeeDTO> GetAll(int companyId)
        {
            SQL sql = new SQL();
            sql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            return sql.ContructList<EmployeeDTO>(sql.ExecuteDataSet(GET_ALL));
        }

        internal EmployeeDTO GetById(int employeeId)
        {
            SQL sql = new SQL();
            sql.AddParameter("@EmployeeId", DbType.Int32, ParameterDirection.Input, 0, employeeId);
            return sql.ContructList<EmployeeDTO>(sql.ExecuteDataSet(GET_BY_ID)).FirstOrDefault();
        }

        public async Task<int> SaveTeam(TeamDto dto)
        {
            SQL sql = new SQL();
            int returnValue = 0;
            try
            {
                sql.BeginTransaction();
                sql.NewCommand();
                var employees = String.Join(",", dto.Employees.Select(o => o.EmployeeId));
                sql.AddParameter("@name", DbType.String, ParameterDirection.Input, 0, dto.Name);
                sql.AddParameter("@code", DbType.String, ParameterDirection.Input, 0, dto.Code);
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                sql.AddParameter("@employees", DbType.String, ParameterDirection.Input, 0, employees);
                if (dto.TeamId == 0)
                {
                    sql.AddParameter("@createdBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
                    sql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedOn);
                    sql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, dto.Name);
                    returnValue = await sql.ExecuteNonQueryAsync(TEAM_ADD);
                }
                else
                {
                    sql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);
                    sql.AddParameter("@modifiedON", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);
                    sql.AddParameter("@teamId", DbType.Int32, ParameterDirection.Input, 0, dto.TeamId);
                    returnValue = await sql.ExecuteNonQueryAsync(TEAM_UPD);

                }


                sql.Commit();
                return returnValue;
            }
            catch (Exception ex)
            {
                sql.Rollback();
                throw ex;
            }
        }
        public async Task<IEnumerable<TeamDto>> TeamList(TeamDto dto)
        {
            SQL sql = new SQL();
            try
            {

                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                return await sql.QueryAsync<TeamDto>(TEAM_LIST);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<TeamDto> TeamById(TeamDto dto)
        {
            SQL sql = new SQL();
            try
            {

                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                sql.AddParameter("@teamId", DbType.Int32, ParameterDirection.Input, 0, dto.TeamId);

                var teamDto = await sql.QueryFirstAsync<TeamDto>(TEAM_BYID);
                if (teamDto != null)
                {
                    sql.NewCommand();
                    sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                    sql.AddParameter("@teamId", DbType.Int32, ParameterDirection.Input, 0, dto.TeamId);

                    teamDto.Employees = (await sql.QueryAsync<EmployeeDTO>(TEAM_EMPLOYEES)).ToList();
                    return teamDto;
                }
                else
                {
                    throw new Exception("Could not find team");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> DeleteTeam(TeamDto dto)
        {
            SQL sql = new SQL();
            try
            {
                sql.AddParameter("@teamId", DbType.Int32, ParameterDirection.Input, 0, dto.TeamId);
                sql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);
                sql.AddParameter("@modifiedON", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                return await sql.ExecuteNonQueryAsync(TEAM_DELETE);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Procedures
        const string ADD = "p_employee_ins";
        const string UPDATE = "p_employee_upd";
        const string GET_BY_ID = "p_employee_sellById";
        const string GET_ALL = "p_employee_sellAll";

        const string TEAM_ADD = "p_team_ins";
        const string TEAM_UPD = "p_team_upd";
        const string TEAM_LIST = "p_team_list";
        const string TEAM_BYID = "p_team_byId";
        const string TEAM_EMPLOYEES = "p_team_employees";
        const string TEAM_DELETE = "p_team_delete";


        #endregion
    }
}
