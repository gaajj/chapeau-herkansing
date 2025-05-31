using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.ViewModels.Management;
using ChapeauHerkansing.Repositories.Mappers;

namespace ChapeauHerkansing.Repositories
{
    public class MenuItemRepository
    {
        private readonly string _connectionString;

        public MenuItemRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDatabase");
        }

        public List<MenuItem> GetAllMenuItems()
        {
            string query = @"
                SELECT 
                    id AS MenuItemID,
                    itemName,
                    price,
                    category,
                    isAlcoholic,
                    isDeleted,
                    stockAmount,
                    menuType
                FROM menuItems
                WHERE isDeleted IS NULL OR isDeleted = 0";

            return ExecuteQuery(query, null, null);
        }

        public List<MenuItem> GetMenuItemsByFilter(MenuType menuType, MenuCategory? category, bool includeDeleted = false)
        {
            string query = @"
                SELECT 
                    id AS MenuItemID,
                    itemName,
                    price,
                    category,
                    isAlcoholic,
                    isDeleted,
                    stockAmount,
                    menuType
                FROM menuItems
                WHERE menuType = @menuType";

            if (category != null)
                query += " AND category = @category";

            if (!includeDeleted)
                query += " AND (isDeleted IS NULL OR isDeleted = 0)";

            return ExecuteQuery(query, menuType, category);
        }

        private List<MenuItem> ExecuteQuery(string query, MenuType? menuType, MenuCategory? category)
        {
            List<MenuItem> items = new List<MenuItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                if (menuType != null)
                    command.Parameters.AddWithValue("@menuType", (int)menuType);

                if (category != null)
                    command.Parameters.AddWithValue("@category", category.ToString().ToLower());

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    items.Add(MenuItemMapper.FromReader(reader));
                }

                reader.Close();
            }

            return items;
        }

        public int InsertMenuItem(MenuItemCreateViewModel model)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    string itemQuery = @"
                        INSERT INTO menuItems (itemName, price, category, isAlcoholic, isDeleted, stockAmount, menuType)
                        OUTPUT INSERTED.id
                        VALUES (@name, @price, @category, @isAlcoholic, 0, @stockAmount, @menuType)";

                    SqlCommand itemCmd = new SqlCommand(itemQuery, conn, transaction);
                    itemCmd.Parameters.AddWithValue("@name", model.Name);
                    itemCmd.Parameters.AddWithValue("@price", model.Price);
                    itemCmd.Parameters.AddWithValue("@category", model.Category.ToString().ToLower());
                    itemCmd.Parameters.AddWithValue("@isAlcoholic", model.IsAlcoholic);
                    itemCmd.Parameters.AddWithValue("@stockAmount", model.StockAmount);
                    itemCmd.Parameters.AddWithValue("@menuType", (int)model.MenuType);

                    int menuItemId = (int)itemCmd.ExecuteScalar();

                    transaction.Commit();
                    return menuItemId;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public MenuItem GetMenuItemById(int id)
        {
            string query = @"
                SELECT 
                    id AS MenuItemID,
                    itemName,
                    price,
                    category,
                    isAlcoholic,
                    isDeleted,
                    stockAmount,
                    menuType
                FROM menuItems
                WHERE id = @id";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    return MenuItemMapper.FromReader(reader);
                return null;
            }
        }

        public void UpdateMenuItem(int id, MenuItemCreateViewModel model)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string updateItem = @"
                    UPDATE menuItems
                    SET itemName = @name,
                        price = @price,
                        category = @category,
                        isAlcoholic = @isAlcoholic,
                        stockAmount = @stockAmount,
                        menuType = @menuType
                    WHERE id = @id";

                SqlCommand itemCmd = new SqlCommand(updateItem, conn);
                itemCmd.Parameters.AddWithValue("@name", model.Name);
                itemCmd.Parameters.AddWithValue("@price", model.Price);
                itemCmd.Parameters.AddWithValue("@category", model.Category.ToString().ToLower());
                itemCmd.Parameters.AddWithValue("@isAlcoholic", model.IsAlcoholic);
                itemCmd.Parameters.AddWithValue("@stockAmount", model.StockAmount);
                itemCmd.Parameters.AddWithValue("@menuType", (int)model.MenuType);
                itemCmd.Parameters.AddWithValue("@id", id);
                itemCmd.ExecuteNonQuery();
            }
        }

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

                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    return (bool)result;
                }

                throw new Exception("Item niet gevonden of geen status gewijzigd.");
            }
        }

        public List<MenuItem> GetMenuItemsByMenuType(MenuType menuType)
        {
            string query = @"
                SELECT 
                    id AS MenuItemID,
                    itemName,
                    price,
                    category,
                    isAlcoholic,
                    isDeleted,
                    stockAmount,
                    menuType
                FROM menuItems
                WHERE menuType = @menuType
                AND (isDeleted IS NULL OR isDeleted = 0)";

            List<MenuItem> items = new List<MenuItem>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@menuType", (int)menuType);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    items.Add(MenuItemMapper.FromReader(reader));
                }

                reader.Close();
            }

            return items;
        }
    }
}
