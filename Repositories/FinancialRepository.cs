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

        public List<FinancialData> GetFinancialData(DateTime fromDate, DateTime toDate, MenuType? filter)
        {
            List<FinancialData> list = new List<FinancialData>();

            using SqlConnection conn = CreateConnection();

            string query = @"
                SELECT 
                    mi.menuType, 
                    COUNT(*) AS TotalSales, 
                    SUM(mi.price) AS Revenue,
                    SUM(p.tip) AS Tips,
                    SUM(mi.price + p.tip) AS TotalIncome
                FROM orders o
                JOIN orderLines ol ON o.id = ol.orderId
                JOIN menuItems mi ON ol.menuItemId = mi.id
                JOIN payments p ON o.id = p.orderId
                WHERE o.orderTime BETWEEN @from AND @to AND o.isDeleted = 0 AND p.isDeleted = 0";

            if (filter != null)
            {
                query += " AND mi.menuType = @menuType";
            }

            query += " GROUP BY mi.menuType";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@from", fromDate);
            cmd.Parameters.AddWithValue("@to", toDate);
            if (filter != null)
            {
                cmd.Parameters.AddWithValue("@menuType", (int)filter);
            }

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
