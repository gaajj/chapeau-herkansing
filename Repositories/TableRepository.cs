using Microsoft.Data.SqlClient;
using System.Configuration;
using ChapeauHerkansing.Models;

namespace ChapeauHerkansing.Repositories
{
    public class TableRepository
    {
        private readonly string connectionString;

        // Constructor voor ophalen van connectiestring
        public TableRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ChapeauDatabase");

        }

        // Methode om alle tafels op te halen
        public List<Table> GetAllTables()
        {
            List<Table> tables = new List<Table>();

            string query = "SELECT id, staffId, seats, tableStatus FROM dbo.tables";


            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                int tableId = reader.GetInt32(0);
                int? staffId = reader.IsDBNull(1) ? null : reader.GetInt32(1);
                int? seats = reader.IsDBNull(2) ? null : reader.GetInt32(2);
                string statusStr = reader.GetString(3);
                TableStatus status = Enum.Parse<TableStatus>(statusStr, true);

                tables.Add(new Table(tableId, null, seats, status));
            }

            return tables;
        }
    }
}
