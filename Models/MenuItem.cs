namespace ChapeauHerkansing.Models
{
    public class MenuItem
    {
        public int MenuItemID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public bool IsAlcoholic { get; set; }

        public MenuItem(int menuItemID, string name, decimal price, string category, bool isAlcoholic)
        {
            MenuItemID = menuItemID;
            Name = name;
            Price = price;
            Category = category;
            IsAlcoholic = isAlcoholic;
        }
        public MenuItem(int menuItemID, string name)
        {
            MenuItemID = menuItemID;
            Name = name;
        }
    }
}
