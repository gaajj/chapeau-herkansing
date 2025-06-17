using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.ViewModels.Management;
using ChapeauHerkansing.Repositories.Readers;
using ChapeauHerkansing.Repositories.Interfaces;

namespace ChapeauHerkansing.Repositories
{
    public class MenuItemRepository : BaseRepository, IMenuItemRepository
    {
        public MenuItemRepository(IConfiguration configuration) : base(configuration) {}

        // Haal alle menu-items op (inclusief soft-deleted)
        public Menu GetAllMenuItems()
        {
            string query = @"
                SELECT id AS MenuItemID, itemName, price, category, isAlcoholic, isDeleted, stockAmount, menuType
                FROM menuItems";

            return ExecuteMenuQuery(query, null, null);
        }

        // Haal menu-items op gefilterd op type en optioneel categorie
        public Menu GetMenuItemsByFilter(MenuType menuType, MenuCategory? category)
        {
            string query = @"
                SELECT id AS MenuItemID, itemName, price, category, isAlcoholic, isDeleted, stockAmount, menuType
                FROM menuItems
                WHERE menuType = @menuType
                AND (@category IS NULL OR category = @category)";

            return ExecuteMenuQuery(query, menuType, category);
        }

        // Herbruikbare methode voor het uitvoeren van menuqueries
        private Menu ExecuteMenuQuery(string query, MenuType? menuType, MenuCategory? category)
        {
            List<MenuItem> items = new();

            using SqlConnection conn = CreateConnection();
            {
                SqlCommand command = new(query, conn);

                if (menuType.HasValue)
                    command.Parameters.AddWithValue("@menuType", (int)menuType.Value);

                command.Parameters.AddWithValue("@category", category == null
                    ? DBNull.Value
                    : category.Value.ToString().ToLower());

                conn.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                    items.Add(MenuItemReader.Read(reader));
            }

            return new Menu(items);
        }

        // Voeg een nieuw item toe
        public int InsertMenuItem(MenuItemCreateViewModel model)
        {
            try
            {
                using SqlConnection conn = CreateConnection();
                {
                    conn.Open();

                    string query = @"
                        INSERT INTO menuItems (itemName, price, category, isAlcoholic, isDeleted, stockAmount, menuType)
                        OUTPUT INSERTED.id
                        VALUES (@name, @price, @category, @isAlcoholic, 0, @stockAmount, @menuType)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", model.Name);
                        cmd.Parameters.AddWithValue("@price", model.Price);
                        cmd.Parameters.AddWithValue("@category", model.Category.ToString().ToLower());
                        cmd.Parameters.AddWithValue("@isAlcoholic", model.IsAlcoholic);
                        cmd.Parameters.AddWithValue("@stockAmount", model.StockAmount);
                        cmd.Parameters.AddWithValue("@menuType", (int)model.MenuType);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while inserting menu item.", ex);
            }
        }

        // Haal één item op via ID
        public MenuItem GetMenuItemById(int id)
        {
            string query = @"
                SELECT id AS MenuItemID, itemName, price, category, isAlcoholic, isDeleted, stockAmount, menuType
                FROM menuItems
                WHERE id = @id";

            using SqlConnection conn = CreateConnection();
            {
                SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                    return MenuItemReader.Read(reader);
                else
                    throw new Exception("Menu-item not found.");
            }
        }

        // Update een menu-item
        public void UpdateMenuItem(int id, MenuItemCreateViewModel model)
        {
            try
            {
                using SqlConnection conn = CreateConnection();
                {
                    conn.Open();

                    string query = @"
                        UPDATE menuItems
                        SET itemName = @name, price = @price, category = @category, 
                            isAlcoholic = @isAlcoholic, stockAmount = @stockAmount, menuType = @menuType
                        WHERE id = @id";

                    SqlCommand cmd = new(query, conn);
                    cmd.Parameters.AddWithValue("@name", model.Name);
                    cmd.Parameters.AddWithValue("@price", model.Price);
                    cmd.Parameters.AddWithValue("@category", model.Category.ToString().ToLower());
                    cmd.Parameters.AddWithValue("@isAlcoholic", model.IsAlcoholic);
                    cmd.Parameters.AddWithValue("@stockAmount", model.StockAmount);
                    cmd.Parameters.AddWithValue("@menuType", (int)model.MenuType);
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while updating menu item.", ex);
            }
        }

        // Toggle soft delete
        public bool ToggleActive(int id)
        {
            using SqlConnection conn = CreateConnection();
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

                SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                    return reader.GetBoolean(reader.GetOrdinal("isDeleted"));

                throw new Exception("Item not found or status not changed.");
            }
        }

        public Menu GetMenuItemsByMenuType(MenuType menuType)
        {
            string query = @"
        SELECT id AS MenuItemID, itemName, price, category, isAlcoholic, isDeleted, stockAmount, menuType
        FROM menuItems
        WHERE menuType = @menuType";

            return ExecuteMenuQuery(query, menuType, null);
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
