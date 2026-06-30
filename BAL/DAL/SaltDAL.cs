using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using BAL.DTO;
namespace BAL.DAL
{
    internal class SaltDAL
    {
        public bool Save(SaltDTO obj)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@Name", DbType.String, ParameterDirection.Input, 0, obj.Name);
            objSql.AddParameter("@Indications", DbType.String, ParameterDirection.Input, 0, obj.Indications);
            objSql.AddParameter("@Dosage", DbType.String, ParameterDirection.Input, 0, obj.Dosage);
            objSql.AddParameter("@SideEffects", DbType.String, ParameterDirection.Input, 0, obj.SideEffects);
            objSql.AddParameter("@Precautions", DbType.String, ParameterDirection.Input, 0, obj.Precautions);
            objSql.AddParameter("@DrugInstructions", DbType.String, ParameterDirection.Input, 0, obj.DrugInstructions);
            objSql.AddParameter("@Note", DbType.String, ParameterDirection.Input, 0, obj.Note);
            objSql.AddParameter("@Narcotic", DbType.Boolean, ParameterDirection.Input, 0, obj.Narcotic);
            objSql.AddParameter("@SCH_H", DbType.Boolean, ParameterDirection.Input, 0, obj.SCH_H);
            objSql.AddParameter("@SCH_H1", DbType.Boolean, ParameterDirection.Input, 0, obj.SCH_H1);


            if (obj.SaltId == 0)
            {
                objSql.AddParameter("@CreatedBy", DbType.Int32, ParameterDirection.Input, 0, obj.CreatedBy);
                objSql.AddParameter("@StoreId", DbType.Int32, ParameterDirection.Input, 0, obj.StoreId);
                objSql.ExecuteNonQuery(ADD);
            }
            else
            {
                objSql.AddParameter("@SaltId", DbType.Int32, ParameterDirection.Input, 0, obj.SaltId);
                objSql.ExecuteNonQuery(UPDATE);
            }

            return true;
        }

        public List<SaltDTO> GetAll(int storeId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@StoreId", DbType.Int32, ParameterDirection.Input, 0, storeId);
            return objSql.ContructList<SaltDTO>(objSql.ExecuteDataSet(GETALL));
        }
        public SaltDTO Get(int saltId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@saltId", DbType.Int32, ParameterDirection.Input, 0, saltId);
            return objSql.ContructList<SaltDTO>(objSql.ExecuteDataSet(GET)).FirstOrDefault();
        }

        public bool ActivateDeActivate(SaltStatus status, int saltId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@saltId", DbType.Int32, ParameterDirection.Input, 0, saltId);
            objSql.AddParameter("@status", DbType.Int32, ParameterDirection.Input, 0, Convert.ToInt16(status));
            return objSql.ExecuteNonQuery(UPDATESTATUS) == 1;

        }

        #region StoredProcedures
        const string ADD = "p_Salt_ins";
        const string UPDATE = "p_Salt_upd";
        const string GETALL = "p_Salt_getAll";
        const string GET = "p_Salt_sel";
        const string UPDATESTATUS = "p_saltUpdateStatus";
        #endregion
    }
}
