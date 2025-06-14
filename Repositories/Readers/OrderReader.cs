using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using Microsoft.Data.SqlClient;

namespace ChapeauHerkansing.Repositories.Readers
{
    public static class OrderReader
    {
        public static Order Read(SqlDataReader reader)
        {
            return new Order(
                reader.GetInt32(reader.GetOrdinal("orderId")),
                TableReader.Read(reader),
                reader.GetBoolean(reader.GetOrdinal("isDeleted")),
                reader.GetDateTime(reader.GetOrdinal("OrderTime"))
            ); 
        }

        public static Order ReadWithLines(SqlDataReader reader)
        {
            Order order = OrderReader.Read(reader);

            do
            {
                if (!reader.IsDBNull(reader.GetOrdinal("orderLineId")))
                {
                    OrderLine orderLine = OrderLineReader.Read(reader, order);
                    order.OrderLines.Add(orderLine);
                }
            } while (reader.Read() && reader.GetInt32(reader.GetOrdinal("orderId")) == order.OrderID);

            return order;
        }
    }
}
