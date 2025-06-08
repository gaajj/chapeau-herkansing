using ChapeauHerkansing.Models;
using Microsoft.Data.SqlClient;
using ChapeauHerkansing.Repositories.Readers;

namespace ChapeauHerkansing.Repositories
{
    public class StaffRepository : BaseRepository
    {
        public StaffRepository(IConfiguration configuration) : base(configuration) { }

        // Haalt alle medewerkers op, optioneel inclusief gedeactiveerden
        public List<Staff> GetAllStaff(bool includeDeleted = false)
        {
            List<Staff> staffList = new();
            using SqlConnection connection = CreateConnection();

            string query = "SELECT ID, firstName, lastName, username, password, role, isDeleted FROM dbo.staff";
            if (!includeDeleted)
                query += " WHERE isDeleted = 0";

            using SqlCommand command = new(query, connection);
            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
                staffList.Add(StaffReader.Read(reader));

            return staffList;
        }


        // Haalt één medewerker op op basis van ID
        public Staff GetStaffById(int id)
        {
            using SqlConnection conn = CreateConnection();
            string query = "SELECT ID, firstName, lastName, username, password, role, isDeleted FROM dbo.staff WHERE ID = @id";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            return reader.Read() ? StaffReader.Read(reader) : null;
        }

        // Voegt een nieuwe medewerker toe
        public void AddStaff(Staff staff)
        {
            using SqlConnection conn = CreateConnection();
            string query = @"
                INSERT INTO dbo.staff (firstName, lastName, username, password, role, isDeleted)
                VALUES (@firstName, @lastName, @username, @password, @role, @isDeleted)";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@firstName", staff.FirstName);
            cmd.Parameters.AddWithValue("@lastName", staff.LastName);
            cmd.Parameters.AddWithValue("@username", staff.Username);
            cmd.Parameters.AddWithValue("@password", staff.Password);
            cmd.Parameters.AddWithValue("@role", staff.Role.ToString());
            cmd.Parameters.AddWithValue("@isDeleted", staff.IsDeleted);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // Update bestaande medewerker
        public void UpdateStaff(Staff staff)
        {
            using SqlConnection conn = CreateConnection();
            string query = @"
                UPDATE dbo.staff
                SET firstName = @firstName, lastName = @lastName, username = @username,
                    password = @password, role = @role, isDeleted = @isDeleted
                WHERE ID = @id";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@firstName", staff.FirstName);
            cmd.Parameters.AddWithValue("@lastName", staff.LastName);
            cmd.Parameters.AddWithValue("@username", staff.Username);
            cmd.Parameters.AddWithValue("@password", staff.Password);
            cmd.Parameters.AddWithValue("@role", staff.Role.ToString());
            cmd.Parameters.AddWithValue("@isDeleted", staff.IsDeleted);
            cmd.Parameters.AddWithValue("@id", staff.Id);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // Zet medewerker aan/uit (soft delete)
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

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Type-safe read of the returned value
                            bool isDeleted = reader.GetBoolean(reader.GetOrdinal("isDeleted"));
                            return isDeleted;
                        }

                        // No rows affected  throw meaningful exception
                        throw new Exception("Staff member not found or status was not changed.");
                    }
                }
            }
        }


        // Haalt medewerker op via gebruikersnaam (voor login)
        public Staff? GetStaffByUsername(string username)
        {
            using SqlConnection conn = CreateConnection();
            string query = @"
                SELECT ID, firstName, lastName, username, password, role, isDeleted
                FROM dbo.staff
                WHERE username = @username AND (isDeleted = 0 OR isDeleted IS NULL)";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@username", username);

            conn.Open();
            using SqlDataReader rdr = cmd.ExecuteReader();
            return rdr.Read() ? StaffReader.Read(rdr) : null;
        }
    }
}
