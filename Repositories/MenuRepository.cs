using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories.Readers;
using ChapeauHerkansing.Repositories.Mappers;
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
            int menuId = reader.GetInt32(reader.GetOrdinal("menuID"));
            string type = reader.GetString(reader.GetOrdinal("menuType"));
            Menu menu = new Menu(menuId, type);

            do
            {
                if (!reader.IsDBNull(reader.GetOrdinal("menuItemID")))
                {
                    MenuItem menuItem = MenuItemMapper.FromReader(reader);
                    menu.MenuItems.Add(menuItem);
                }
            } while (reader.Read() && reader.GetInt32(reader.GetOrdinal("menuID")) == menuId);

            return menu;
        }
    }
}
