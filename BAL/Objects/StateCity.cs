using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DAL;
namespace BAL.Objects
{
    public class State
    {
        /// <summary>
        /// Select States 
        /// </summary>
        /// <returns>StateDTO</returns>
        public static List<StateDTO> GetAllStates()
        {
            StateCityDAL objDal = new StateCityDAL();
            return objDal.GetAllStates();
        }

        /// <summary>
        /// Select cities of state
        /// </summary>
        /// <param name="stateId">StateId</param>
        /// <returns>CityDTO</returns>
        public static List<CityDTO> GetCites(int stateId)
        {
            StateCityDAL objDal = new StateCityDAL();
            return objDal.GetAllCities(stateId);
        }
    }

    
}
