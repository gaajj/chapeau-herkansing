using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using Microsoft.Data.SqlClient;

namespace ChapeauHerkansing.Repositories.Readers
{
    public static class StaffReader
    {
        public static Staff Read(SqlDataReader reader)
        {
            return new Staff(
                reader.GetInt32(reader.GetOrdinal("staffId")),
                reader.GetString(reader.GetOrdinal("firstName")),
                reader.GetString(reader.GetOrdinal("lastName")),
                reader.GetString(reader.GetOrdinal("username")),
                reader.GetString(reader.GetOrdinal("password")),
               
             Enum.TryParse<Role>(reader.GetString(reader.GetOrdinal("role")), out var parsedRole)
                ? parsedRole
                : throw new Exception($"Ongeldige rolwaarde in database: {reader.GetString(reader.GetOrdinal("role"))}"));
            
        }
    }
}