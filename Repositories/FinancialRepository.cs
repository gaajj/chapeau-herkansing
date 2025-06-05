using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace ChapeauHerkansing.Repositories
{
    public class FinancialRepository : BaseRepository
    {
        public FinancialRepository(IConfiguration configuration) : base(configuration) { }

        public List<FinancialData> GetFinancialData(DateTime startDate, DateTime endDate)
        {
            List<FinancialData> result = new List<FinancialData>();

            string query = @"
                SELECT mi.menuType, COUNT(ol.id) AS TotalSales, SUM(mi.price * ol.amount) AS TotalIncome
                FROM orderLines ol
                INNER JOIN menuItems mi ON ol.menuItemId = mi.id
                INNER JOIN orders o ON ol.orderId = o.id
                WHERE o.orderTime BETWEEN @startDate AND @endDate AND o.isDeleted = 0
                GROUP BY mi.menuType";

            using (SqlConnection connection = CreateConnection())
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@startDate", startDate);
                command.Parameters.AddWithValue("@endDate", endDate);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int menuTypeInt = Convert.ToInt32(reader["menuType"]);
                    string menuTypeName = Enum.IsDefined(typeof(MenuType), menuTypeInt)
                        ? ((MenuType)menuTypeInt).ToString()
                        : "Unknown";

                    int totalSales = Convert.ToInt32(reader["TotalSales"]);
                    decimal totalIncome = Convert.ToDecimal(reader["TotalIncome"]);

                    FinancialData data = new FinancialData
                    {
                        MenuType = menuTypeName,
                        TotalSales = totalSales,
                        TotalIncome = totalIncome
                    };

                    result.Add(data);
                }

                reader.Close();
            }

            return result;
        }

        public decimal GetTotalTipAmount(DateTime startDate, DateTime endDate)
        {
            decimal totalTips = 0;

            string query = @"
                SELECT SUM(p.tip)
                FROM payments p
                INNER JOIN orders o ON p.orderId = o.id
                WHERE o.orderTime BETWEEN @startDate AND @endDate AND o.isDeleted = 0";

            using (SqlConnection connection = CreateConnection())
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@startDate", startDate);
                command.Parameters.AddWithValue("@endDate", endDate);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read() && !reader.IsDBNull(0))
                {
                    totalTips = reader.GetDecimal(0);
                }

                reader.Close();
            }

            return totalTips;
        }
    }
}
