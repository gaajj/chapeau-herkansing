using ChapeauHerkansing.Models.Enums;

namespace ChapeauHerkansing.Models
{
    public class MenuItem
    {
        public int MenuItemID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public MenuCategory Category { get; set; }
        public bool IsAlcoholic { get; set; }
        public bool? IsDeleted { get; set; }
        public int StockAmount { get; set; } // Rechtstreeks op MenuItem

        public MenuType MenuType { get; set; } // Via JOIN opgehaald

        // Constructors
        public MenuItem(int menuItemID, string name, decimal price, MenuCategory category, bool isAlcoholic)
        {
            MenuItemID = menuItemID;
            Name = name;
            Price = price;
            Category = category;
            IsAlcoholic = isAlcoholic;
        }

        public MenuItem(int menuItemID, string name, decimal price, MenuCategory category, bool isAlcoholic, bool? isDeleted, int stockAmount, MenuType menuType)
        {
            MenuItemID = menuItemID;
            Name = name;
            Price = price;
            Category = category;
            IsAlcoholic = isAlcoholic;
            IsDeleted = isDeleted;
            StockAmount = stockAmount;
            MenuType = menuType;
        }

    }
}
