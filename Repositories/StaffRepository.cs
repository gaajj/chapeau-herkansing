using ChapeauHerkansing.Models;
using Microsoft.Data.SqlClient;
using ChapeauHerkansing.Repositories.Readers;
using ChapeauHerkansing.Repositories.Interfaces;

namespace ChapeauHerkansing.Repositories
{
    public class StaffRepository : BaseRepository, IStaffRepository
    {
        public StaffRepository(IConfiguration configuration) : base(configuration) { }

        // Haalt alle medewerkers op (optioneel inclusief gedeactiveerden)
        public List<Staff> GetAllStaff(bool includeDeleted = false)
        {
            List<Staff> staffList = new();
            using SqlConnection connection = CreateConnection();

            string query = "SELECT ID, firstName, lastName, username, password, role, isDeleted FROM dbo.staff";
            if (!includeDeleted)
                query += " WHERE isDeleted = 0";

            using SqlCommand command = new(query, connection);
            try
            {
                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    staffList.Add(StaffReader.Read(reader));
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Database error while retrieving staff list.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error while retrieving staff.", ex);
            }

            return staffList;
        }

        // Haalt één medewerker op via ID
        public Staff GetStaffById(int id)
        {
            using SqlConnection conn = CreateConnection();
            string query = "SELECT ID, firstName, lastName, username, password, role, isDeleted FROM dbo.staff WHERE ID = @id";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            try
            {
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                return reader.Read() ? StaffReader.Read(reader) : null;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Database error while retrieving staff member by ID.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error while retrieving staff by ID.", ex);
            }
        }

        // Voegt nieuwe medewerker toe aan de database
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

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Database error while adding staff member.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error while adding staff member.", ex);
            }
        }

        // Werkt gegevens van een bestaande medewerker bij
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

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Database error while updating staff member.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error while updating staff.", ex);
            }
        }

        // Zet medewerker aan/uit (soft delete toggle)
        public bool ToggleStaffActive(int id)
        {
            using SqlConnection conn = CreateConnection();
            string query = @"
                UPDATE dbo.staff
                SET isDeleted = CASE WHEN isDeleted IS NULL OR isDeleted = 0 THEN 1 ELSE 0 END
                OUTPUT INSERTED.isDeleted
                WHERE ID = @id";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            try
            {
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    bool isDeleted = reader.GetBoolean(reader.GetOrdinal("isDeleted"));
                    return isDeleted;
                }

                throw new Exception("Staff member not found or no status was changed.");
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Database error while toggling staff active status.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error while toggling staff status.", ex);
            }
        }

        // Login: zoek medewerker op gebruikersnaam
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
