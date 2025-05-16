using System.Collections.Generic;
using ChapeauHerkansing.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ChapeauHerkansing.Repositories
{
    public class StaffRepository : BaseRepository, IStaffRepository
    {
        public StaffRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public List<Staff> GetAllStaff()
        {
            List<Staff> staffList = new List<Staff>();

            SqlConnection connection = CreateConnection();
            SqlCommand command = new SqlCommand("SELECT ID, firstName, lastName, username, password, role, isDeleted FROM dbo.staff", connection);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Staff staffMember = MapToStaff(reader);
                staffList.Add(staffMember);
            }

            reader.Close();
            connection.Close();

            return staffList;
        }

        // Deze methode leest 1 rij uit SqlDataReader en zet deze om in een Staff-object
        private Staff MapToStaff(SqlDataReader reader)
        {
            int id = reader.GetInt32(0);
            string firstName = reader.GetString(1);
            string lastName = reader.GetString(2);
            string username = reader.GetString(3);
            string password = reader.GetString(4);
            string role = reader.GetString(5);
            bool? isDeleted = reader.IsDBNull(6) ? (bool?)null : reader.GetBoolean(6);

            return new Staff(id, firstName, lastName, username, password, role, isDeleted);
        }

        // Tijdelijke lege methodes voor interface-compatibiliteit (voor Sprint 2)
        public Staff GetStaffById(int id)
        {
            return null;
        }

        public void AddStaff(Staff staff)
        {
        }

        public void UpdateStaff(Staff staff)
        {
        }

        public void DeleteStaff(int id)
        {
        }
    }
}
