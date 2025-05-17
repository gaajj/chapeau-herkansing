using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using Microsoft.Data.SqlClient;
using System;

namespace ChapeauHerkansing.Repositories.Mappers
{
    public static class StaffMapper
    {
        public static Staff FromReader(SqlDataReader reader)
        {
            int id = reader.GetInt32(0);
            string firstName = reader.GetString(1);
            string lastName = reader.GetString(2);
            string username = reader.GetString(3);
            string password = reader.GetString(4);
            string roleString = reader.GetString(5);
            bool isDeleted = !reader.IsDBNull(6) && reader.GetBoolean(6);

            Role role = Enum.TryParse<Role>(roleString, out var parsedRole)
                ? parsedRole
                : throw new Exception($"Ongeldige rolwaarde in database: {roleString}");

            return new Staff(id, firstName, lastName, username, password, role, isDeleted);
        }
    }
}
