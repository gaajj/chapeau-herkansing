using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using Microsoft.Data.SqlClient;

namespace ChapeauHerkansing.Repositories.Readers
{
    public static class OrderLineReader
    {
        public static OrderLine Read(SqlDataReader reader, Order order)
        {
            return new OrderLine(
                reader.GetInt32(reader.GetOrdinal("orderLineId")),
                order,
                MenuItemReader.Read(reader),
                StaffReader.Read(reader),
                reader.GetInt32(reader.GetOrdinal("stockAmount")),
                reader.GetDateTime(reader.GetOrdinal("OrderTime")),
                reader.IsDBNull(reader.GetOrdinal("note")) ? null : reader.GetString(reader.GetOrdinal("note")),
                ParseOrderStatus(reader.GetString(reader.GetOrdinal("orderStatus")))
            );
        }

        private static OrderStatus ParseOrderStatus(string status)
        {
            return status.ToLower() switch
            {
                "ordered" => OrderStatus.Ordered,
                "ready" => OrderStatus.Ready,
                "served" => OrderStatus.Served,
                _ => throw new InvalidCastException($"Unknown orderStatus: {status}")
            };
        }
    }
}
