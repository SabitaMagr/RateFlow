using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Assignment.Data
{
    public class ConnectionManager
    {
         IConfiguration _configuration;

        // Inject IConfiguration via constructor
        public ConnectionManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Method to retrieve the DefaultConnection string
        public SqlConnectionStringBuilder GetConnectionString()
        {
            // Get the connection string from appsettings.json
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            return new SqlConnectionStringBuilder(connectionString);
        }
    }
}
