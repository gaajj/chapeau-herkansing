using ChapeauHerkansing.Models;
using Microsoft.Data.SqlClient;

namespace ChapeauHerkansing.Repositories
{
    public class StaffRepository : IRepository<Staff>
    {
        private readonly string _connectionString;

        public StaffRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDatabase");
        }

        public List<Staff> GetAll()
        { 


        List<Staff> staffList = new List<Staff>(); 

      using (SqlConnection connection = new SqlConnection(_connectionString))
      {
        string query = "SELECT id, firstName, lastName, username, password, role, isDeleted FROM dbo.staff";
        SqlCommand command = new SqlCommand(query, connection);

        connection.Open();
        SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
          Staff staff = ReadTable(reader);
          staffList.Add(staff);
        }
      }
      return staffList;
    }

    private Staff ReadTable(SqlDataReader reader) {
      int id = reader.GetInt32(0);
      string firstName = reader.GetString(1);
      string lastName = reader.GetString(2);
      string username = reader.GetString(3);
      string password = reader.GetString(4);
      string role = reader.GetString(5);

      return new Staff(id, firstName, lastName, username, password, role);
    } 
  }
}
