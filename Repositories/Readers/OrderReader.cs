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
                reader.GetDateTime(reader.GetOrdinal("timeCreated")) 


            ); 
        }
    }
}
