using System;
using Microsoft.Data.SqlClient;

namespace ChapeauHerkansing.Repositories
{
    public class StockRepository
    {
        private readonly string _connectionString;

        public StockRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDatabase");
        }

        public int InsertStock(int amount)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO stock (amount) OUTPUT INSERTED.id VALUES (@amount)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@amount", amount);
                return (int)cmd.ExecuteScalar();
            }
        }

        public void UpdateStock(int menuItemId, int newAmount)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
                    UPDATE s
                    SET s.amount = @amount
                    FROM stock s
                    JOIN menuItems m ON s.id = m.stockId
                    WHERE m.id = @menuItemId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@amount", newAmount);
                cmd.Parameters.AddWithValue("@menuItemId", menuItemId);
                cmd.ExecuteNonQuery();
            }
        }

        public int? GetStockAmount(int menuItemId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT s.amount
                    FROM stock s
                    JOIN menuItems m ON s.id = m.stockId
                    WHERE m.id = @menuItemId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@menuItemId", menuItemId);

                object result = cmd.ExecuteScalar();
                return result == DBNull.Value || result == null ? null : (int?)result;
            }
        }
    }
}
