using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DbCommunication
{
    public interface IDbConnection
    {
        int KioskId { get; set; }

        int InsertRow(string query);
        int InsertRow(string query, List<SqlParameter> parameters);
        int InsertRow(string query, List<SqlParameter> parameters, SqlTransaction transaction);

        long InsertRowLong(string query, List<SqlParameter> parameters);
        long InsertRowLong(string query, List<SqlParameter> parameters, SqlTransaction transaction);
        long InsertRowLong(string query, List<SqlParameter> parameters, SqlTransaction transaction, SqlConnection connection);

        int UpdateRow(string query);
        int UpdateRow(string query, List<SqlParameter> parameters);
        int UpdateRow(string query, List<SqlParameter> parameters, SqlTransaction transaction);

        object GetScalar(string query);
        object GetScalar(string query, List<SqlParameter> parameters);

        DataTable GetTable(string query);
        DataTable GetTable(string query, List<SqlParameter> parameters);

        SqlTransaction BeginTransaction();
    }
}
