using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories.Readers;
using ChapeauHerkansing.Repositories.Mappers;
using Microsoft.Data.SqlClient;
using ChapeauHerkansing.Repositories.Readers.ChapeauHerkansing.Repositories.Readers;
using ChapeauHerkansing.Models.Enums;

namespace ChapeauHerkansing.Repositories
{
    public class MenuRepository : BaseRepository
    {
        public MenuRepository(IConfiguration configuration) : base(configuration) { }

        public MenuItem GetMenuItemById(int menuItemId)
        {
            string query = @"
                SELECT mi.id AS menuItemId,
                mi.itemName,
                mi.price,
                mi.category,
                mi.isAlcoholic,
                mi.isDeleted,
                s.id AS stockId,
                s.amount
                FROM
                    menuItems mi
                LEFT JOIN
                    stock s ON mi.id = s.id
                WHERE
                    mi.id = @menuItemId;";

            var parameters = new Dictionary<string, object>
            {
                { "@menuItemId", menuItemId }
            };

            return ExecuteSingle(query, MenuItemReader.Read, parameters);
        }

        public Menu? GetFilteredMenu(string menuType, MenuCategory? category = null)
        {
            string query = @"
                SELECT 
                    m.id AS menuID, 
                    m.menuType,
                    mi.id AS menuItemID,
                    mi.itemName,
                    mi.price,
                    mi.category,
                    mi.isAlcoholic,
                    s.amount
                FROM 
                    dbo.menu m
                LEFT JOIN 
                    dbo.menu_menuItems mmi ON m.id = mmi.menuId
                LEFT JOIN 
                    dbo.menuItems mi ON mi.id = mmi.menuItemId
                LEFT JOIN 
                    dbo.stock s ON s.id = mi.stockid
                WHERE 
                    m.menuType = @MenuType 
                    AND (@Category = '' OR mi.category = @Category)
                    AND mi.isDeleted = 0
                ORDER BY 
                    mi.category, mi.itemName;
            ";

            var parameters = new Dictionary<string, object>
            {
                { "@MenuType", menuType },
                { "@Category", category.ToString() }
            };

            return ExecuteQuery(query, ReadMenuWithItems, parameters).FirstOrDefault();
        }

        public List<string> GetAllCategories()
        {
            string query = @"
                SELECT DISTINCT 
                    mi.category
                FROM 
                    dbo.menuItems mi
                WHERE 
                    mi.isDeleted = 0
                ORDER BY 
                    mi.category;
            ";
            return ExecuteQuery(query, reader => reader.GetString(0));
        }

        private Menu ReadMenuWithItems(SqlDataReader reader)
        {
            int menuId = reader.GetInt32(reader.GetOrdinal("menuID"));
            string type = reader.GetString(reader.GetOrdinal("menuType"));
            Menu menu = new Menu(menuId, type);

            do
            {
                if (!reader.IsDBNull(reader.GetOrdinal("menuItemID")))
                {
                    MenuItem menuItem = MenuItemReader.Read(reader);
                    menu.MenuItems.Add(menuItem);
                }
            } while (reader.Read() && reader.GetInt32(reader.GetOrdinal("menuID")) == menuId);

            return menu;
        }
    }
}
