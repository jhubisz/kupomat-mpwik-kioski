using System.Configuration;

namespace DbCommunication
{
    public class DbLocalConfiguration : IDbConfiguration
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
