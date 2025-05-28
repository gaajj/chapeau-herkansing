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
                    m.id AS MenuItemID,
                    m.itemName,
                    m.price,
                    m.category,
                    m.isAlcoholic,
                    m.isDeleted,
                    m.stockAmount,
                    mm.menuId,
                    mt.menuType
                FROM menuItems m
                JOIN menu_menuItems mm ON m.id = mm.menuItemId
                JOIN menu mt ON mm.menuId = mt.id
                WHERE m.isDeleted IS NULL OR m.isDeleted = 0";

            return ExecuteQuery(query, null, null);
        }

        public List<MenuItem> GetMenuItemsByFilter(MenuType menuType, MenuCategory? category, bool includeDeleted = false)
        {
            string query = @"
                SELECT 
                    m.id AS MenuItemID,
                    m.itemName,
                    m.price,
                    m.category,
                    m.isAlcoholic,
                    m.isDeleted,
                    m.stockAmount,
                    mm.menuId,
                    mt.menuType
                FROM menuItems m
                JOIN menu_menuItems mm ON m.id = mm.menuItemId
                JOIN menu mt ON mm.menuId = mt.id
                WHERE mt.menuType = @menuType";

            if (category != null)
                query += " AND m.category = @category";

            if (!includeDeleted)
                query += " AND (m.isDeleted IS NULL OR m.isDeleted = 0)";

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
                        INSERT INTO menuItems (itemName, price, category, isAlcoholic, isDeleted, stockAmount)
                        OUTPUT INSERTED.id
                        VALUES (@name, @price, @category, @isAlcoholic, 0, @stockAmount)";

                    SqlCommand itemCmd = new SqlCommand(itemQuery, conn, transaction);
                    itemCmd.Parameters.AddWithValue("@name", model.Name);
                    itemCmd.Parameters.AddWithValue("@price", model.Price);
                    itemCmd.Parameters.AddWithValue("@category", model.Category.ToString().ToLower());
                    itemCmd.Parameters.AddWithValue("@isAlcoholic", model.IsAlcoholic);
                    itemCmd.Parameters.AddWithValue("@stockAmount", model.StockAmount);

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
                    m.id AS MenuItemID,
                    m.itemName,
                    m.price,
                    m.category,
                    m.isAlcoholic,
                    m.isDeleted,
                    m.stockAmount,
                    mm.menuId,
                    mt.menuType
                FROM menuItems m
                JOIN menu_menuItems mm ON m.id = mm.menuItemId
                JOIN menu mt ON mm.menuId = mt.id
                WHERE m.id = @id";

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
                        stockAmount = @stockAmount
                    WHERE id = @id";

                SqlCommand itemCmd = new SqlCommand(updateItem, conn);
                itemCmd.Parameters.AddWithValue("@name", model.Name);
                itemCmd.Parameters.AddWithValue("@price", model.Price);
                itemCmd.Parameters.AddWithValue("@category", model.Category.ToString().ToLower());
                itemCmd.Parameters.AddWithValue("@isAlcoholic", model.IsAlcoholic);
                itemCmd.Parameters.AddWithValue("@stockAmount", model.StockAmount);
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
    }
}
