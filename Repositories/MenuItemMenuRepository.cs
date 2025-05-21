using Microsoft.Data.SqlClient;

namespace ChapeauHerkansing.Repositories
{
    public class MenuItemMenuRepository
    {
        private readonly string _connectionString;

        public MenuItemMenuRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDatabase");
        }

        public void LinkMenuItemToMenu(int menuId, int menuItemId, SqlConnection? connection = null, SqlTransaction? transaction = null)
        {
            bool externalConnection = connection != null;

            if (!externalConnection)
            {
                connection = new SqlConnection(_connectionString);
                connection.Open();
            }

            try
            {
                string query = "INSERT INTO menu_menuItems (menuId, menuItemId) VALUES (@menuId, @menuItemId)";
                SqlCommand cmd = new SqlCommand(query, connection, transaction);
                cmd.Parameters.AddWithValue("@menuId", menuId);
                cmd.Parameters.AddWithValue("@menuItemId", menuItemId);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (!externalConnection)
                    connection?.Close();
            }
        }
    }
}
