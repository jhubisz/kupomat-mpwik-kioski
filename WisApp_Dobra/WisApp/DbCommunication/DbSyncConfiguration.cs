using System.Configuration;

namespace DbCommunication
{
    public class DbSyncConfiguration : IDbConfiguration
    {
        public string ServerAddress
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["mssql"].ConnectionString;
            }
        }
    }
}
