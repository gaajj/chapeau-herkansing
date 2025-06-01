using ChapeauHerkansing.Models;
using Microsoft.Data.SqlClient;
using ChapeauHerkansing.Repositories.Readers;

namespace ChapeauHerkansing.Repositories
{
    public class StaffRepository : BaseRepository
    {
        public StaffRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public StaffCollection GetAllStaff(bool includeDeleted = false)
        {
            List<Staff> staffList = new List<Staff>();
            SqlConnection connection = CreateConnection();

            string query = "SELECT ID, firstName, lastName, username, password, role, isDeleted FROM dbo.staff";
            if (!includeDeleted)
            {
                query += " WHERE isDeleted = 0";
            }

            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                staffList.Add(StaffReader.Read(reader));
            }

            reader.Close();
            connection.Close();

            return new StaffCollection(staffList);
        }


        public Staff GetStaffById(int id)
        {
            using (SqlConnection conn = CreateConnection())
            {
                string query = "SELECT ID, firstName, lastName, username, password, role, isDeleted FROM dbo.staff WHERE ID = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return StaffReader.Read(reader);
                }

                return null;
            }
        }

        public void AddStaff(Staff staff)
        {
            using (SqlConnection conn = CreateConnection())
            {
                string query = @"INSERT INTO dbo.staff (firstName, lastName, username, password, role, isDeleted)
                                 VALUES (@firstName, @lastName, @username, @password, @role, @isDeleted)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@firstName", staff.FirstName);
                cmd.Parameters.AddWithValue("@lastName", staff.LastName);
                cmd.Parameters.AddWithValue("@username", staff.Username);
                cmd.Parameters.AddWithValue("@password", staff.Password);
                cmd.Parameters.AddWithValue("@role", staff.Role.ToString());
                cmd.Parameters.AddWithValue("@isDeleted", staff.IsDeleted); 

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateStaff(Staff staff)
        {
            using (SqlConnection conn = CreateConnection())
            {
                string query = @"UPDATE dbo.staff
                                 SET firstName = @firstName,
                                     lastName = @lastName,
                                     username = @username,
                                     password = @password,
                                     role = @role,
                                     isDeleted = @isDeleted
                                 WHERE ID = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@firstName", staff.FirstName);
                cmd.Parameters.AddWithValue("@lastName", staff.LastName);
                cmd.Parameters.AddWithValue("@username", staff.Username);
                cmd.Parameters.AddWithValue("@password", staff.Password);
                cmd.Parameters.AddWithValue("@role", staff.Role.ToString()); cmd.Parameters.AddWithValue("@isDeleted", staff.IsDeleted); 
                cmd.Parameters.AddWithValue("@id", staff.Id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public bool ToggleStaffActive(int id)
        {
            using (SqlConnection conn = CreateConnection())
            {
                string query = @"
                    UPDATE dbo.staff
                    SET isDeleted = CASE
                        WHEN isDeleted IS NULL OR isDeleted = 0 THEN 1
                        ELSE 0
                    END
                    OUTPUT INSERTED.isDeleted
                    WHERE ID = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    return (bool)result;
                }

                throw new Exception("Medewerker niet gevonden of status kon niet gewijzigd worden.");
            }
        }
    }
}
