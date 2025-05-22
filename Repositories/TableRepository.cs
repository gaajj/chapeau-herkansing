using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using ChapeauHerkansing.Models;

namespace ChapeauHerkansing.Repositories
{
    public class TableRepository
    {
        private readonly string _connectionString;
        public TableRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("ChapeauDatabase");
        }

        public List<Table> GetAllTables()
        {
            var tables = new List<Table>();
            const string sql = @"
                SELECT id, staffId, seats, tableStatus
                FROM dbo.tables";
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var tableId = reader.GetInt32(0);
                var seats = reader.IsDBNull(2) ? null : (int?)reader.GetInt32(2);
                var status = Enum.Parse<TableStatus>(reader.GetString(3), true);
                tables.Add(new Table(tableId, null, seats, status));
            }
            return tables;
        }

        public int GetReadyOrdersCount(int tableId)
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM dbo.orderLines ol
                JOIN dbo.orders o ON ol.orderId = o.id
                WHERE o.tableId    = @tableId
                  AND ol.orderStatus = 'ready'
                  AND ol.isDeleted   = 0";
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@tableId", tableId);
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public void ServeOrdersForTable(int tableId)
        {
            const string sql = @"
                UPDATE ol
                SET orderStatus = 'served'
                FROM dbo.orderLines ol
                JOIN dbo.orders o ON ol.orderId = o.id
                WHERE o.tableId     = @tableId
                  AND ol.orderStatus = 'ready'
                  AND ol.isDeleted   = 0";
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@tableId", tableId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public bool HasUnfinishedOrders(int tableId)
        {
            const string sql = @"
        SELECT COUNT(*) 
        FROM dbo.orderLines l
        JOIN dbo.orders o ON o.id = l.orderId
        WHERE o.tableId = @tableId
          AND l.orderStatus <> 'Served'";
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@tableId", tableId);
            conn.Open();
            var count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

        public void UpdateTableStatus(int tableId, TableStatus status)
        {
            const string sql = "UPDATE dbo.tables SET tableStatus = @status WHERE id = @tableId";
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@tableId", tableId);
            cmd.Parameters.AddWithValue("@status", status.ToString().ToLower());
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public List<string> GetRunningOrderStatuses(int tableId)
        {
            const string sql = @"
        SELECT DISTINCT ol.orderStatus
          FROM dbo.orderLines ol
          JOIN dbo.orders o ON ol.orderId = o.id
         WHERE o.tableId      = @tableId
           AND ol.orderStatus <> 'served'
           AND ol.isDeleted   = 0";
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@tableId", tableId);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            var list = new List<string>();
            while (reader.Read())
                list.Add(reader.GetString(0));
            return list;
        }

        public void SetTableFree(int tableId)
        {
            UpdateTableStatus(tableId, TableStatus.Free);
        }



    }
}
