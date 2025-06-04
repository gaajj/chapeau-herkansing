using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Repositories.Interfaces;
using ChapeauHerkansing.Repositories.Readers;

namespace ChapeauHerkansing.Repositories
{
    public class TableRepository : BaseRepository, ITableRepository
    {
        public TableRepository(IConfiguration config) : base(config)
        {
        }

        public Table? GetTableById(int tableId)
        {
            string query = @"
                SELECT t.id AS tableId, t.staffId, t.seats, t.tableStatus 
                FROM tables t
                JOIN staff s ON s.id = t.staffId
                WHERE t.id = @tableId;
            ";

            var parameters = new Dictionary<string, object>
            {
                { "@tableId", tableId }
            };

            return ExecuteSingle(query, TableReader.Read, parameters);
        }

        public List<Table> GetAllTables()
        {
            List<Table> tables = new List<Table>();
            const string sql = @"
        SELECT id, staffId, seats, tableStatus
        FROM dbo.tables";
            SqlConnection conn = GetConnection();
            SqlCommand cmd = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int tableId = reader.GetInt32(0);
                int? seats = reader.IsDBNull(2) ? null : (int?)reader.GetInt32(2);
                TableStatus status = Enum.Parse<TableStatus>(reader.GetString(3), true);
                tables.Add(new Table(tableId, null, seats, status));
            }
            return tables;
        }



        public int GetReadyOrdersCount(int tableId, string orderStatus, bool includeDeleted) // geen magic waardes meer
        {
            const string sql = @"
        SELECT COUNT(*)
        FROM dbo.orderLines ol
        JOIN dbo.orders o ON ol.orderId = o.id
        WHERE o.tableId       = @tableId
          AND ol.orderStatus  = @orderStatus
          AND ol.isDeleted    = @isDeleted";

            using SqlConnection conn = GetConnection();
            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@tableId", tableId);
            cmd.Parameters.AddWithValue("@orderStatus", orderStatus);
            cmd.Parameters.AddWithValue("@isDeleted", includeDeleted ? 1 : 0);

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
        WHERE o.tableId       = @tableId
          AND ol.orderStatus  = 'ready'
          AND ol.isDeleted    = 0";
            using var conn = GetConnection();
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
        WHERE o.tableId        = @tableId
          AND l.orderStatus   <> 'served'";
            using var conn = GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@tableId", tableId);
            conn.Open();
            var count = (int)cmd.ExecuteScalar();
            return count > 0;
        }


        public void UpdateTableStatus(int tableId, TableStatus status)
        {
            const string sql = @"
        UPDATE dbo.tables
        SET tableStatus = @status
        WHERE id = @tableId";
            using var conn = GetConnection();
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
        WHERE o.tableId        = @tableId
          AND ol.orderStatus  <> 'served'
          AND ol.isDeleted    = 0";
            using var conn = GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@tableId", tableId);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            var list = new List<string>();
            while (reader.Read())
            {
                list.Add(reader.GetString(0));
            }
            return list;
        }


        public void SetTableFree(int tableId) // in de controller vrij zetten
        {
            UpdateTableStatus(tableId, TableStatus.Free);
        }



    }
}
