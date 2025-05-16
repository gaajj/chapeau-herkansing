using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ChapeauHerkansing.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly string connectionString;

        protected BaseRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ChapeauDatabase");
        }

        protected SqlConnection CreateConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
