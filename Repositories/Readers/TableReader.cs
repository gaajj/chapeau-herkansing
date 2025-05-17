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
                StaffReader.Read(reader),
                reader.GetInt32(reader.GetOrdinal("seats")),
                reader.GetString(reader.GetOrdinal("tableStatus"))
            );
        }
    }
}
