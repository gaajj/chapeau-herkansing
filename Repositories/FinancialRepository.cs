using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using Microsoft.Data.SqlClient;
using System;

namespace ChapeauHerkansing.Repositories
{
    public class FinancialRepository : BaseRepository
    {
        public FinancialRepository(IConfiguration configuration) : base(configuration) { }

        public List<FinancialData> GetFinancialData(DateTime fromDate, DateTime toDate)
        {
            List<FinancialData> list = new List<FinancialData>();

            using SqlConnection conn = CreateConnection();

            string query = @"
            SELECT  mi.menuType, COUNT(*) AS TotalSales, 
            SUM(mi.price) AS Revenue,
            SUM(p.tip) AS Tips,
            SUM(mi.price + p.tip) AS TotalIncome
                FROM orders o
                JOIN orderLines ol ON o.id = ol.orderId
                JOIN menuItems mi ON ol.menuItemId = mi.id
                JOIN payments p ON o.id = p.orderId
                WHERE p.isDeleted = 0
                GROUP BY mi.menuType";

            using SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                FinancialData data = new FinancialData
                {
                    MenuType = Enum.GetName(typeof(MenuType), reader.GetInt32(0)),
                    TotalSales = reader.GetInt32(1),
                    Revenue = reader.GetDecimal(2),
                    Tips = reader.GetDecimal(3),
                    TotalIncome = reader.GetDecimal(4)
                };

                list.Add(data);
            }

            return list;
        }
    }
}
