using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.ViewModels.Management;
using ChapeauHerkansing.Repositories.Readers;

namespace ChapeauHerkansing.Repositories
{
    public class MenuItemRepository : BaseRepository
    {
        private readonly string _connectionString;

        public MenuItemRepository(IConfiguration configuration) : base(configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDatabase");
        }

        // Haal alle zichtbare (niet verwijderde) menu-items op
        public Menu GetAllMenuItems()
        {
            string query = @"
            SELECT id AS MenuItemID, itemName, price, category, isAlcoholic, isDeleted, stockAmount, menuType
            FROM menuItems
            WHERE isDeleted IS NULL OR isDeleted = 0";

            return ExecuteMenuQuery(query, null, null);
        }

        // Haal menu-items op op basis van type en categorie, eventueel inclusief verwijderde items
        public Menu GetMenuItemsByFilter(MenuType menuType, MenuCategory? category, bool includeDeleted = false)
        {
            string query = @"
            SELECT id AS MenuItemID, itemName, price, category, isAlcoholic, isDeleted, stockAmount, menuType
            FROM menuItems
            WHERE menuType = @menuType";

            if (category != null)
                query += " AND category = @category";

            if (!includeDeleted)
                query += " AND (isDeleted IS NULL OR isDeleted = 0)";

            return ExecuteMenuQuery(query, menuType, category);
        }

        // Herbruikbare methode voor menu-queries met optionele filters
        private Menu ExecuteMenuQuery(string query, MenuType? menuType, MenuCategory? category)
        {
            List<MenuItem> items = new();

            using (SqlConnection connection = new(_connectionString))
            {
                SqlCommand command = new(query, connection);

                if (menuType != null)
                    command.Parameters.AddWithValue("@menuType", (int)menuType);
                if (category != null)
                    command.Parameters.AddWithValue("@category", category.ToString().ToLower());

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                    items.Add(MenuItemReader.Read(reader));

                reader.Close();
            }

            return new Menu(items); // Geef een Menu-object terug (met lijst van items)
        }

        // Voeg een nieuw menu-item toe aan de database en retourneer het nieuwe ID
        public int InsertMenuItem(MenuItemCreateViewModel model)
        {
            try
            {
                using (SqlConnection conn = new(_connectionString))
                {
                    conn.Open();

                    string itemQuery = @"
                    INSERT INTO menuItems (itemName, price, category, isAlcoholic, isDeleted, stockAmount, menuType)
                    OUTPUT INSERTED.id
                    VALUES (@name, @price, @category, @isAlcoholic, 0, @stockAmount, @menuType)";

                    using (SqlCommand itemCmd = new SqlCommand(itemQuery, conn))
                    {
                        // Voeg parameters toe (voorkomt SQL-injectie)
                        itemCmd.Parameters.AddWithValue("@name", model.Name);
                        itemCmd.Parameters.AddWithValue("@price", model.Price);
                        itemCmd.Parameters.AddWithValue("@category", model.Category.ToString().ToLower());
                        itemCmd.Parameters.AddWithValue("@isAlcoholic", model.IsAlcoholic);
                        itemCmd.Parameters.AddWithValue("@stockAmount", model.StockAmount);
                        itemCmd.Parameters.AddWithValue("@menuType", (int)model.MenuType);

                        int menuItemId = (int)itemCmd.ExecuteScalar(); // Geef het ID van het nieuw aangemaakte item terug
                        return menuItemId;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Databasefout, bijvoorbeeld constraint violation of verbinding mislukt
                throw new Exception("Error while trying to save menu item to database.", sqlEx);
            }
            catch (Exception ex)
            {
                // Andere fout, zoals null reference of conversieprobleem
                throw new Exception("Unexpected error while adding menu item.", ex);
            }
        }

        // Haal één menu-item op via ID
        public MenuItem GetMenuItemById(int id)
        {
            string query = @"
            SELECT id AS MenuItemID, itemName, price, category, isAlcoholic, isDeleted, stockAmount, menuType
            FROM menuItems
            WHERE id = @id";

            using (SqlConnection conn = new(_connectionString))
            {
                SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                return reader.Read() ? MenuItemReader.Read(reader) : null; // Als gevonden, lees en retourneer het item
            }
        }

        // Pas een bestaand menu-item aan
        public void UpdateMenuItem(int id, MenuItemCreateViewModel model)
        {
            try
            {
                using (SqlConnection conn = new(_connectionString))
                {
                    conn.Open();

                    string updateItem = @"
                    UPDATE menuItems
                    SET itemName = @name, price = @price, category = @category, 
                        isAlcoholic = @isAlcoholic, stockAmount = @stockAmount, menuType = @menuType
                    WHERE id = @id";

                    SqlCommand itemCmd = new(updateItem, conn);
                    itemCmd.Parameters.AddWithValue("@name", model.Name);
                    itemCmd.Parameters.AddWithValue("@price", model.Price);
                    itemCmd.Parameters.AddWithValue("@category", model.Category.ToString().ToLower());
                    itemCmd.Parameters.AddWithValue("@isAlcoholic", model.IsAlcoholic);
                    itemCmd.Parameters.AddWithValue("@stockAmount", model.StockAmount);
                    itemCmd.Parameters.AddWithValue("@menuType", (int)model.MenuType);
                    itemCmd.Parameters.AddWithValue("@id", id);

                    itemCmd.ExecuteNonQuery(); // Voer update uit
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error while updating menu item in database.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error while updating menu item.", ex);
            }
        }

        // Activeer of deactiveer een item (soft delete)
        public bool ToggleActive(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                UPDATE menuItems
                SET isDeleted = CASE 
                    WHEN isDeleted IS NULL OR isDeleted = 0 THEN 1 
                    ELSE 0 
                END
                OUTPUT INSERTED.isDeleted
                WHERE id = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // Haal de nieuwe waarde van isDeleted op en retourneer die
                    bool isDeleted = reader.GetBoolean(reader.GetOrdinal("isDeleted"));
                    return isDeleted;
                }

                // Als geen rij is aangepast, dan was het ID ongeldig
                throw new Exception("Item not found or no status was changed.");
            }
        }

        // Haal menu-items op op basis van alleen menuType
        public Menu GetMenuItemsByMenuType(MenuType menuType)
        {
            string query = @"
            SELECT id AS MenuItemID, itemName, price, category, isAlcoholic, isDeleted, stockAmount, menuType
            FROM menuItems
            WHERE menuType = @menuType AND (isDeleted IS NULL OR isDeleted = 0)";

            Menu menu = new();

            using (SqlConnection conn = new(_connectionString))
            {
                SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@menuType", (int)menuType);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                    menu.MenuItems.Add(MenuItemReader.Read(reader));

                reader.Close();
            }

            return menu;
        }

        // Update de voorraad van een item
        public void UpdateStock(int menuItemId, int amountChange)
        {
            string query = @"
                UPDATE menuItems
                SET stockAmount = stockAmount + @change
                WHERE id = @id AND (isDeleted IS NULL OR isDeleted = 0)";

            try
            {
                using SqlConnection connection = CreateConnection();
                using SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@change", amountChange);
                command.Parameters.AddWithValue("@id", menuItemId);

                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error while updating stock amount.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error while updating stock.", ex);
            }
        }
    }
}
