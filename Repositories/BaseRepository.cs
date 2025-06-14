using Microsoft.Data.SqlClient;

namespace ChapeauHerkansing.Repositories
{

    public abstract class BaseRepository
    {
        private readonly string _connectionString;

        protected BaseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDatabase");
        }

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }


        protected void ExecuteNonQuery(string query, Dictionary<string, object>? paraments = null)
        {
            using (SqlConnection connection = GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    AddParameters(command, paraments);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }


       
        protected SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        protected T? ExecuteSingle<T>(string query, Func<SqlDataReader, T> mapFunction, Dictionary<string, object>? parameters = null) where T : class
        {
            using (var connection = GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    AddParameters(command, parameters);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        return reader.Read() ? mapFunction(reader) : null;
                    }
                }
            }
        }

        protected List<T> ExecuteQuery<T>(string query, Func<SqlDataReader, T> mapFunction, Dictionary<string, object>? parameters = null)
        {
            var results = new List<T>();
            using (SqlConnection connection = GetConnection())
            {
                using (var command = new SqlCommand(query, connection))
                {
                    AddParameters(command, parameters);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(mapFunction(reader));
                        }
                    }
                }
            }
            return results;
        }

        protected List<T> ExecuteGroupedQuery<T>(string query, Func<SqlDataReader, Dictionary<int, T>, T> mapFunction, Dictionary<string, object>? parameters = null)
        {
            Dictionary<int, T> grouped = new Dictionary<int, T>();

            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                AddParameters(command, parameters);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mapFunction(reader, grouped);
                    }
                }
            }

            return grouped.Values.ToList();
        }

        private void AddParameters(SqlCommand command, Dictionary<string, object>? parameters)
        {
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            }
        }
    }
}
