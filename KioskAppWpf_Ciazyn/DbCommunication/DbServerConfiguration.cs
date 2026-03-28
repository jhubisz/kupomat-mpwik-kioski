using System.Configuration;

namespace DbCommunication
{
    public class DbServerConfiguration : IDbConfiguration
    {
        public string ServerAddress
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["mssql_server"].ConnectionString;
            }
        }
    }
}
