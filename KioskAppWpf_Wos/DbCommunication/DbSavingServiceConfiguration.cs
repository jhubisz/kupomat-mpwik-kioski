using System.Configuration;

namespace DbCommunication
{
    public class DbSavingServiceConfiguration : IDbConfiguration
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
