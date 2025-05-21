using ChapeauHerkansing.Models;
using Microsoft.Data.SqlClient;

namespace ChapeauHerkansing.Repositories.Readers
{
    public static class TableReader
    {
        public static Table Read(SqlDataReader reader)
        {
            return new Table(
                reader.GetInt32(reader.GetOrdinal("tableId")),
              null,
                reader.GetInt32(reader.GetOrdinal("seats")),
                 Enum.Parse<TableStatus>(reader.GetString(reader.GetOrdinal("tableStatus")), true)); }
          
           
        
    }
}
