using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DbCommunication
{
    public class DbConnection : IDbConnection
    {
        private SqlConnection connection;
        public SqlConnection Connection
        {
            get
            {
                if (connection == null || connection.State != ConnectionState.Open)
                {
                    OpenConnection();
                }
                return connection;
            }
        }
        public IDbConfiguration Configuration { get; set; }
        public int KioskId { get; set; }

        #region Singleton implementation
        private static Dictionary<IDbConfiguration, DbConnection> instance;
        public static DbConnection GetInstance(IDbConfiguration configuration, int kioskId)
        {
            if (instance == null)
                instance = new Dictionary<IDbConfiguration, DbConnection>();

            if (!instance.ContainsKey(configuration))
                instance.Add(configuration, null);

            if (instance[configuration] == null)
            {
                instance[configuration] = new DbConnection();
                instance[configuration].KioskId = kioskId;
                instance[configuration].Configuration = configuration;
                instance[configuration].OpenConnection();
            }
            return instance[configuration];
        }
        public static void CloseInstance(IDbConfiguration configuration)
        {
            if (instance.ContainsKey(configuration))
            {
                instance[configuration].connection.Close();
                instance.Remove(configuration);
            }
        }

        private DbConnection()
        {
        }
        #endregion

        private void OpenConnection()
        {
            connection = new SqlConnection(Configuration.ServerAddress);
            connection.Open();
        }

        public int InsertRow(string query)
        {
            return InsertRow(query, null);
        }
        public int InsertRow(string query, List<SqlParameter> parameters)
        {
            return InsertRow(query, parameters, null);
        }
        public int InsertRow(string query, List<SqlParameter> parameters, SqlTransaction transaction)
        {
            SqlCommand command = new SqlCommand(query, Connection);
            if (parameters != null && parameters.Count > 0)
            {
                foreach (var par in parameters)
                    command.Parameters.Add(par);
            }

            if (transaction != null)
                command.Transaction = transaction;

            var resultInt = 0;
            try
            {
                resultInt = (int)command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                SimpleLoggerDb sl = new SimpleLoggerDb(Configuration);
                sl.Error(ex.Message);
                sl.Error(query);
                sl.Error(String.Join<SqlParameter>(",", parameters));
                //sl.Error(ex.StackTrace);
            }

            return resultInt;
        }

        public int UpdateRow(string query)
        {
            return UpdateRow(query, null, null);
        }
        public int UpdateRow(string query, List<SqlParameter> parameters)
        {
            return UpdateRow(query, parameters, null);
        }
        public int UpdateRow(string query, List<SqlParameter> parameters, SqlTransaction transaction)
        {
            try
            {
                SqlCommand command = new SqlCommand(query, Connection);
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (var par in parameters)
                        command.Parameters.Add(par);
                }

                if (transaction != null)
                    command.Transaction = transaction;

                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                SimpleLoggerDb sl = new SimpleLoggerDb(Configuration);
                sl.Error(ex.Message);
                sl.Error(ex.StackTrace);
            }
            return 0;
        }

        public object GetScalar(string query)
        {
            return GetScalar(query, null);
        }
        public object GetScalar(string query, List<SqlParameter> parameters)
        {
            try
            {
                SqlCommand command = new SqlCommand(query, Connection);

                if (parameters != null && parameters.Count > 0)
                {
                    foreach (var par in parameters)
                        command.Parameters.Add(par);
                }

                return command.ExecuteScalar();
            }
            catch(Exception ex)
            {
                SimpleLoggerDb sl = new SimpleLoggerDb(Configuration);
                sl.Error(ex.Message);
                sl.Error(ex.StackTrace);
            }

            return null;
        }

        public DataTable GetTable(string query)
        {
            return GetTable(query, null);
        }
        public DataTable GetTable(string query, List<SqlParameter> parameters)
        {
            try
            {
                SqlCommand command = new SqlCommand(query, Connection);

                if (parameters != null && parameters.Count > 0)
                {
                    foreach (var par in parameters)
                        command.Parameters.Add(par);
                }

                var table = new DataTable();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    table.Load(reader);
                }

                return table;
            }
            catch (Exception ex)
            {
                SimpleLoggerDb sl = new SimpleLoggerDb(Configuration);
                sl.Error(ex.Message);
                sl.Error(ex.StackTrace);
            }
            return null;
        }

        public SqlTransaction BeginTransaction()
        {
            return Connection.BeginTransaction();
        }
    }
}
