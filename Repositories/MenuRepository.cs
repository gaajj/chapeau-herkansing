using ChapeauHerkansing.Models;
using Microsoft.Data.SqlClient;

namespace ChapeauHerkansing.Repositories
{
    public class MenuRepository : BaseRepository
    {
        public MenuRepository(IConfiguration configuration) : base(configuration) { }

        public List<Menu> GetAll()
        {
            List<Menu> menus = new List<Menu>();
            return menus;
        }

        public Menu? GetFilteredMenu(string menuType)
        {
            string query = @"
                SELECT 
                    m.id AS MenuID, 
                    m.menuType AS Type,
                    mi.id AS MenuItemID,
                    mi.itemName AS Name,
                    mi.price AS Price,
                    mi.category AS Category,
                    mi.isAlcoholic AS IsAlcoholic,
                    s.amount AS StockAmount
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
                    AND mi.isDeleted = 0
                ORDER BY 
                    mi.category, mi.itemName;
            ";

            var parameters = new Dictionary<string, object>
            {
                { "@MenuType", menuType }
            };

            return ExecuteQuery(query, ReadMenuWithItems, parameters).FirstOrDefault();
        }

        private Menu ReadMenuWithItems(SqlDataReader reader)
        {
            int menuId = reader.GetInt32(reader.GetOrdinal("MenuID"));
            string type = reader.GetString(reader.GetOrdinal("Type"));
            Menu menu = new Menu(menuId, type);

            do
            {
                if (!reader.IsDBNull(reader.GetOrdinal("MenuItemID")))
                {
                    MenuItem menuItem = ReadMenuItem(reader);
                    menu.MenuItems.Add(menuItem);
                }
            } while (reader.Read() && reader.GetInt32(reader.GetOrdinal("MenuID")) == menuId);

            return menu;
        }

        private MenuItem ReadMenuItem(SqlDataReader reader)
        {
            return new MenuItem(
                reader.GetInt32(reader.GetOrdinal("MenuItemID")),
                reader.GetString(reader.GetOrdinal("Name")),
                reader.GetDecimal(reader.GetOrdinal("Price")),
                reader.GetString(reader.GetOrdinal("Category")),
                reader.GetBoolean(reader.GetOrdinal("IsAlcoholic"))
                );
        }
    }
}
