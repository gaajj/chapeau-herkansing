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

        public User? GetByLoginCredentials(string username, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT ID, firstName, lastName, username, password, role 
                                 FROM dbo.staff 
                                 WHERE username = @username AND password = @password";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return ReadUser(reader);
                }
            }

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
