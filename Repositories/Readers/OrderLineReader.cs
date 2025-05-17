using ChapeauHerkansing.Models;
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
                reader.GetInt32(reader.GetOrdinal("amount")),
                reader.GetDateTime(reader.GetOrdinal("orderTime")),
                reader.IsDBNull(reader.GetOrdinal("note")) ? null : reader.GetString(reader.GetOrdinal("note"))
            );
        }
    }
}
