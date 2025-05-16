using ChapeauHerkansing.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ChapeauHerkansing.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string _connectionString;

        public UsersRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDatabase");
        }

        public List<User> GetAll()
        {
            var userList = new List<User>();

            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT ID, firstName, lastName, username, password, role FROM dbo.staff";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    User user = ReadUser(reader);
                    userList.Add(user);
                }
            }

            return userList;
        }

        public User? GetByUsername(string username)
        {
            using var conn = new SqlConnection(_connectionString);
            const string sql = @"SELECT ID, firstName, lastName, username, password, role
                         FROM dbo.staff
                         WHERE username = @username";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", username);
            conn.Open();
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read()) return ReadUser(rdr);
            return null;
        }


        private User ReadUser(SqlDataReader reader)
        {
            int id = reader.GetInt32(0);
            string firstName = reader.GetString(1);
            string lastName = reader.GetString(2);
            string username = reader.GetString(3);
            string password = reader.GetString(4);
            string role = reader.GetString(5);

            return new User(id, firstName, lastName, username, password, role);
        }
    }
}
