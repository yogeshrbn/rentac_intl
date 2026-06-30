using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using System.Data;
namespace BAL.DAL
{
    public class StateCityDAL
    {

        /// <summary>
        /// Get States
        /// </summary>
        /// <returns>StateDTO</returns>
        public List<StateDTO> GetAllStates()
        {
            SQL objSql = new SQL();
            return objSql.ContructList<StateDTO>(objSql.ExecuteDataSet(GET_ALL_STATES));
        }

        /// <summary>
        /// Select city of given state
        /// </summary>
        /// <param name="stateId">StateId</param>
        /// <returns>CityDTO</returns>
        public List<CityDTO> GetAllCities(int stateId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@stateId", DbType.Int16, ParameterDirection.Input, 0, stateId);
            return objSql.ContructList<CityDTO>(objSql.ExecuteDataSet(GET_ALL_CITIES));
        }
        #region StoredProcedures
        const string GET_ALL_STATES = "p_States_All";
        const string GET_ALL_CITIES = "p_Cities";
        #endregion
    }
}
