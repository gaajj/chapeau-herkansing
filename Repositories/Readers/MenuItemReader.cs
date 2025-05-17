using ChapeauHerkansing.Models;
using Microsoft.Data.SqlClient;

namespace ChapeauHerkansing.Repositories.Readers
{
    public static class MenuItemReader
    {
        public static MenuItem Read(SqlDataReader reader)
        {
            return new MenuItem(
                reader.GetInt32(reader.GetOrdinal("menuItemId")),
                reader.GetString(reader.GetOrdinal("itemName")),
                reader.GetDecimal(reader.GetOrdinal("price")),
                reader.GetString(reader.GetOrdinal("category")),
                reader.GetBoolean(reader.GetOrdinal("isAlcoholic"))
            );
        }
    }
}
