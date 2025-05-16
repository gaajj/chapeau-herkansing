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

        public int StockID { get; set; }       // FK naar stock (bijv. voor voorraad)
        public int? StockAmount { get; set; }

        public MenuType MenuType { get; set; } // FK naar menu (bij JOIN opgehaald)
    }
}
