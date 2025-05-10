using ChapeauHerkansing.Models;
using Microsoft.Data.SqlClient;

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
      var users = new List<User>();

      using (var connection = new SqlConnection(_connectionString))
      {
        string query = "SELECT userID, firstName, lastName, username, password, role FROM dbo.Users";
        SqlCommand command = new SqlCommand(query, connection);

        connection.Open();
        SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
          User user = ReadUser(reader);
          users.Add(user);
        }
      }
      return users;
    }

    private User ReadUser(SqlDataReader reader) {
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
