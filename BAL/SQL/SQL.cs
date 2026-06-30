/*
 * #########################################################################################################################
 * File             :   SQL.cs
 * Details          :   Communicates with the SQL server database and does the various transactions
 * Dependancy       :   Native Ado.Net library
 * Copyright        :   Vinod Chopra Associates
 * Auther           :   Yogesh Sharma 
 * 
 * Development History
 * ---------------------------------------------------------------------
 * Date             Description                 Developer
 * ---------------------------------------------------------------------
 * Jan-07-14        Class created               Yogesh Sharma
 * 
 * #########################################################################################################################
 */


#region Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using static Dapper.SqlMapper;

#endregion

namespace BAL.DAL
{

    /// <summary>
    /// Class SQL does database transactions to the database provided in the connectionstring.
    /// </summary>
    public class SQL : IDisposable
    {

        SqlCommand cmd = null;
        SqlConnection sqlCon = null;
        string sqlSTR = ConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
        List<SqlCommand> commands = new List<SqlCommand>();
        SqlTransaction transaction = null;
        int _commandIndex = 0;

        /// <summary>
        /// Creates a new instance of sqlConnection object and opens a connection to the database from the connection string.
        /// Default connection string is 'sqlCon'
        /// </summary>
        public SQL()
        {
            InitialzeConnection();
        }

        /// <summary>
        /// Creates a new instance of sqlConnection object and opens a connection to the database from the connection string.
        /// </summary>
        /// <param name="connectionStringName">Name of connection string in the connectionstrings section of configuration file.</param>
        public SQL(string connectionStringName)
        {
            sqlSTR = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            InitialzeConnection();
        }


        public void CreateTempTable(string destination, string columns)
        {
            string cmdText = " Create table " + destination + " ( " + columns + " )";
            cmd.CommandText = cmdText;
            cmd.ExecuteNonQuery();
        }


        public void BulkCopy(DataTable source, string destination)
        {

            using (SqlBulkCopy copy = new SqlBulkCopy(sqlCon))
            {
                copy.DestinationTableName = destination;
                copy.WriteToServer(source);
            }
        }
        public DataTable BulkCopy(DataTable source, string destination, string columns, string avgColumns, int itemCategory, int itemType, int userId)
        {
            DataTable table = new DataTable();
            CreateTempTable(destination, columns);
            using (SqlBulkCopy copy = new SqlBulkCopy(sqlCon))
            {
                copy.DestinationTableName = destination;
                copy.WriteToServer(source);
            }
            AddParameter("@CostColumns", DbType.String, ParameterDirection.Input, 1000, avgColumns);
            AddParameter("@userId", DbType.Int16, ParameterDirection.Input, 8, userId);
            AddParameter("@tablName", DbType.String, ParameterDirection.Input, 30, destination);
            //  DataSet ds = ExecuteDataSet("p_GetCostTempData");
            string storedProcs = "";
            if (itemCategory == 1 && itemType != 2) // All wet goods exept GNS
            {
                storedProcs = "p_GetCostTempData_wet";
            }
            else // all dry goods and GNS
            {
                storedProcs = "p_GetCostTempData";
            }
            DataSet ds = ExecuteDataSet(storedProcs);


            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }
            }
            return table;
        }
        /// <summary>
        /// Creates a new instance of sqlConnection object and opens a connection to the database from the connection string.
        /// </summary>
        private void InitialzeConnection()
        {
            if (sqlCon == null)
                sqlCon = new SqlConnection(sqlSTR);

            if (sqlCon.State != ConnectionState.Closed)
                sqlCon.Close();
            sqlCon.Open();
            if (cmd == null)
            {
                NewCommand();
            }
        }

        public void NewCommand()
        {
            //if (commands.Count > 0)
            //    _commandIndex++;
            cmd = null;
            SqlCommand newCmd = new SqlCommand();
            newCmd.CommandTimeout = 120;
            if (transaction != null)
            {
                newCmd.Transaction = transaction;
            }
            newCmd.Connection = sqlCon;
            //commands.Add(newCmd);
            cmd = newCmd;
        }


        /// <summary>
        /// Releases all the resources used.
        /// </summary>
        public void Dispose()
        {
            sqlCon.Dispose();
            cmd.Dispose();
        }

        /// <summary>
        /// Adds a parameter to the SqlCommand Object.
        /// </summary>
        /// <param name="pName">Parameter name</param>
        /// <param name="type">DbType of the parameter</param>
        /// <param name="direction">Direction of the parameter</param>
        /// <param name="size">Size of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        public void AddParameter(string pName, DbType type, ParameterDirection direction, int size, object value)
        {
            DbParameter p = new SqlParameter();
            p.DbType = type;
            p.ParameterName = pName;
            p.Direction = direction;
            p.Size = size;
            p.Value = value;
            cmd.Parameters.Add(p);

        }
        public void AddDynamicParameter(string pName, DbType type, ParameterDirection direction, int size, object value)
        {
            DbParameter p = new SqlParameter();
            p.DbType = type;
            p.ParameterName = pName;
            p.Direction = direction;
            p.Size = size;
            p.Value = value;
            cmd.Parameters.Add(p);

        }
        public void BeginTransaction()
        {
            transaction = sqlCon.BeginTransaction();

        }
        public void BeginTransaction(IsolationLevel level)
        {
            transaction = sqlCon.BeginTransaction(level);

        }
        public void Commit()
        {
            transaction.Commit();
        }
        public void Rollback()
        {
            transaction.Rollback();
        }

        /// <summary>
        /// Executes the stored procedure and returns the dataset.
        /// </summary>
        /// <param name="pName">Stored procedure name</param>
        /// <returns>Dataset object</returns>
        public DataSet ExecuteDataSet(string pName)
        {
            SetCMDName(pName);
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataSet ds = new DataSet();
            da.Fill(ds);
            OutputParameters();
            if (cmd.Transaction == null)
            {
                Dispose();
            }
            return ds;
        }
        /// <summary>
        /// Executes the stored procedure and returns the dataset.
        /// </summary>
        /// <param name="pName">Stored procedure name</param>
        /// <returns>Dataset object</returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string pName)
        {
            //   SetCMDName(pName);

            // SqlDataAdapter da = new SqlDataAdapter();
            //da.SelectCommand = cmd;
            // var read = await cmd.ExecuteReaderAsync();
            var pars = new DynamicParameters();
            foreach(SqlParameter p in cmd.Parameters)
            {
                pars.Add(p.ParameterName, p.Value, p.DbType, p.Direction, p.Size);
            }
            return await sqlCon.QueryAsync<T>(pName, pars, transaction, null, CommandType.StoredProcedure);

        }
        /// <summary>
        /// Executes the stored procedure and returns the dataset.
        /// </summary>
        /// <param name="pName">Stored procedure name</param>
        /// <returns>Dataset object</returns>
        public async Task<T> QueryFirstAsync<T>(string pName)
        {
            //   SetCMDName(pName);

            // SqlDataAdapter da = new SqlDataAdapter();
            //da.SelectCommand = cmd;
            // var read = await cmd.ExecuteReaderAsync();
            var pars = new DynamicParameters();
            foreach (SqlParameter p in cmd.Parameters)
            {
                pars.Add(p.ParameterName, p.Value, p.DbType, p.Direction, p.Size);
            }
             
            return await sqlCon.QueryFirstAsync<T>(pName, pars, transaction, null, CommandType.StoredProcedure);

        }
        /// <summary>
        /// Executes the stored procedure and returns the dataset.
        /// </summary>
        /// <param name="pName">Stored procedure name</param>
        /// <returns>Dataset object</returns>
        public async Task<GridReader> QueryMultipleAsync(string pName)
        {
            //   SetCMDName(pName);

            // SqlDataAdapter da = new SqlDataAdapter();
            //da.SelectCommand = cmd;
            // var read = await cmd.ExecuteReaderAsync();
            var pars = new DynamicParameters();
            foreach (SqlParameter p in cmd.Parameters)
            {
                pars.Add(p.ParameterName, p.Value, p.DbType, p.Direction, p.Size);
            }

            return await sqlCon.QueryMultipleAsync(pName, pars, transaction, null, CommandType.StoredProcedure);

        }
        DbParameter[] param = null;
        /// <summary>
        /// Array of DbParameters
        /// </summary>
        public DbParameter[] OutParams
        {
            get { return param; }
        }

        /// <summary>
        /// Array of Output parameters
        /// </summary>
        /// <returns></returns>
        private DbParameter[] OutputParameters()
        {
            param = new DbParameter[cmd.Parameters.Count];
            int i = 0;
            foreach (DbParameter dp in cmd.Parameters)
            {
                if (dp.Direction == ParameterDirection.Output)
                {
                    param[i] = dp;
                }
                i++;
            }
            return param;
        }

        /// <summary>
        /// Set the command name and Command type to stored procedure of sqlCommand object
        /// </summary>
        /// <param name="pName">Name of stored procedure.</param>
        private void SetCMDName(string pName)
        {
            cmd.CommandText = pName;
            cmd.CommandType = CommandType.StoredProcedure;
        }

        /// <summary>
        /// Executes the stored procedure on the database and returns the output as IDataReader
        /// </summary>
        /// <param name="pName">Name of the Stored procedure</param>
        /// <returns>IDataReader</returns>
        public IDataReader ExecuteReader(string pName)
        {
            SetCMDName(pName);
            IDataReader dr = cmd.ExecuteReader();
            OutputParameters();
            return dr;
        }


        /// <summary>
        /// Executes the stored procedure on the database.
        /// This is basically for DML operations.
        /// </summary>
        /// <param name="pName">Stored procedure name</param>
        /// <returns>Integer value</returns>
        public int ExecuteNonQuery(string pName)
        {
            SetCMDName(pName);
            int value = cmd.ExecuteNonQuery();
            OutputParameters();
            //Dispose();
            return value;
        }
        /// <summary>
        /// Executes the stored procedure on the database.
        /// This is basically for DML operations.
        /// </summary>
        /// <param name="pName">Stored procedure name</param>
        /// <returns>Integer value</returns>
        public async Task<int> ExecuteNonQueryAsync(string pName)
        {
            SetCMDName(pName);
            int value = await cmd.ExecuteNonQueryAsync();
            OutputParameters();
            //Dispose();
            return value;
        }

        /// <summary>
        /// Executes the stored procedure on the database and returns one single value.
        /// </summary>
        /// <param name="pName">Stored procedure name</param>
        /// <returns>vale of type Object.</returns>
        public object ExecuteScalar(string pName)
        {
            SetCMDName(pName);
            object retvalue = cmd.ExecuteScalar();
            OutputParameters();
            //Dispose();
            return retvalue;
        }

        /// <summary>
        /// Executes the stored procedure on the database and returns one single value.
        /// </summary>
        /// <param name="pName">Stored procedure name</param>
        /// <returns>vale of type Object.</returns>
        public async Task<object> ExecuteScalarAsync(string pName)
        {
            SetCMDName(pName);
            object retvalue = await cmd.ExecuteScalarAsync();
            OutputParameters();
            //Dispose();
            return retvalue;
        }

        /// <summary>
        /// Creates a collection of type T from the Dataset passed.
        /// </summary>
        /// <typeparam name="T">Type of which collection is to be created.</typeparam>
        /// <param name="dr">Dataset object.</param>
        /// <returns>List of T.</returns>
        public List<T> ContructList<T>(DataSet dr)
        {
            List<T> objList = new List<T>();
            foreach (DataTable dt in dr.Tables)
            {
                foreach (DataRow row in dt.Rows)
                {
                    T objT;
                    Type t = typeof(T);
                    BindingFlags bflags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;
                    ConstructorInfo cInfo = typeof(T).GetConstructor(bflags, null, new Type[0] { }, null);
                    if (cInfo != null)
                    {
                        objT = (T)cInfo.Invoke(null);
                    }
                    else
                        objT = Activator.CreateInstance<T>();

                    PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));

                    for (int col = 0; col <= dt.Columns.Count - 1; col++)
                    {
                        foreach (PropertyDescriptor prop in properties)
                        {
                            if (prop.Name.ToUpper() == dt.Columns[col].ColumnName.ToUpper() && !string.IsNullOrEmpty(Convert.ToString(row[col])))
                            {
                                try
                                {
                                    if (prop.PropertyType.BaseType.Name == "Enum")
                                    {
                                        EnumConverter eCon = new EnumConverter(prop.PropertyType);
                                        prop.SetValue(objT, eCon.ConvertFromString(Convert.ToString(row[col])));

                                    }

                                    else
                                    {
                                        prop.SetValue(objT, Convert.ChangeType(row[col], prop.PropertyType));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //suppress the exception
                                }
                            }
                        }
                    }

                    objList.Add(objT);
                }
            }
            return objList;


        }




    }
}
