using BAL.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DAL
{
    public class RBNClientDAL
    {
        public RBNClientDTO GetInfo(int clientId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@rbnClientId", DbType.Int32, ParameterDirection.Input, 0, clientId);

            var ds = objSql.ExecuteDataSet(GET_INFO);
            var client = new RBNClientDTO();
            client = (from d in ds.Tables[0].AsEnumerable()
                      select new RBNClientDTO
                      {
                          Name = d.Field<String>("name"),
                          Address1 = d.Field<String>("Address1"),
                          Address2 = d.Field<String>("Address2"),
                          City = d.Field<String>("City"),
                          DefaultCompanyId = d.Field<Int32?>("DefaultCompanyId"),
                          Stateid = d.Field<Int32?>("stateId"),
                          NoGst = d.Field<byte?>("nogst"),
                          PinCode = d.Field<String>("Zipcode"),
                          GST = d.Field<String>("GSTno"),
                          PAN = d.Field<String>("Pan"),
                          SpocName = d.Field<String>("SpocName"),
                          Email = d.Field<String>("Email"),
                          Mobile = d.Field<String>("Mobile")
                      }
                      ).FirstOrDefault();
            return client;
        }
        public int UpdateInfo(RBNClientDTO client)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@rbnClientId", DbType.Int32, ParameterDirection.Input, 0, client.RbnClientId);
            objSql.AddParameter("@Address1", DbType.String, ParameterDirection.Input, 0, client.Address1);
            objSql.AddParameter("@Address2", DbType.String, ParameterDirection.Input, 0, client.Address2);
            objSql.AddParameter("@stateId", DbType.Int32, ParameterDirection.Input, 0, client.Stateid);
            objSql.AddParameter("@City", DbType.String, ParameterDirection.Input, 0, client.City);
            objSql.AddParameter("@pinCode", DbType.Int32, ParameterDirection.Input, 0, client.PinCode);
            objSql.AddParameter("@pan", DbType.String, ParameterDirection.Input, 0, client.PAN);
            objSql.AddParameter("@gst", DbType.String, ParameterDirection.Input, 0, client.GST);
            objSql.AddParameter("@nogst", DbType.Single, ParameterDirection.Input, 0, client.NoGst);
            objSql.AddParameter("@email", DbType.String, ParameterDirection.Input, 0, client.Email);
            objSql.AddParameter("@mobile", DbType.String, ParameterDirection.Input, 0, client.Mobile);
            objSql.AddParameter("@spocName", DbType.String, ParameterDirection.Input, 0, client.SpocName);
            
            objSql.AddParameter("@defaultCompanyId", DbType.Int32, ParameterDirection.Input, 0, client.DefaultCompanyId);

            return objSql.ExecuteNonQuery(UPD_INFO);

        }

        //public int UpdateTaxInfo(RBNClientDTO client)
        //{
        //    SQL objSql = new SQL();
        //    objSql.AddParameter("@rbnClientId", DbType.Int32, ParameterDirection.Input, 0, client.RbnClientId);
        //    objSql.AddParameter("@pan", DbType.String, ParameterDirection.Input, 0, client.PAN);
        //    objSql.AddParameter("@gst", DbType.String, ParameterDirection.Input, 0, client.GST);
        //    objSql.AddParameter("@nogst", DbType.Single, ParameterDirection.Input, 0, client.NoGst);
        //    return objSql.ExecuteNonQuery(UPD_TAXINFO);

        //}

        public int Register(RBNClientDTO client)
        {

            SQL objSql = new SQL();
            try
            {
                objSql.BeginTransaction();
                objSql.NewCommand();
                objSql.AddParameter("@name", DbType.String, ParameterDirection.Input, 0, client.Name);
                objSql.AddParameter("@email", DbType.String, ParameterDirection.Input, 0, client.Email);
                objSql.AddParameter("@mobile", DbType.String, ParameterDirection.Input, 0, client.Mobile);
                objSql.AddParameter("@spocName", DbType.String, ParameterDirection.Input, 0, client.SpocName);
                var clientId = Convert.ToInt32(objSql.ExecuteScalar(CREATE));

                objSql.Commit();
                return clientId;
            }
            catch (Exception ex)
            {
                objSql.Rollback();
                throw ex;
            }

        }
        #region procedures
        const string GET_INFO = "p_createRBNClient_getInfo";
        const string UPD_INFO = "p_RBNClient_updInfo";
        const string CREATE = "p_RbnClients_ins";
        const string CREATE_COMPANY = "p_Company_ins";
        const string CREATE_USER = "p_CreateUser";
        //const string UPD_TAXINFO = "p_RBNClient_updTaxInfo";

        #endregion

    }
}
