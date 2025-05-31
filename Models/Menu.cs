namespace ChapeauHerkansing.Models
{
    public class Menu
    {
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

        public Menu()
        {
            MenuItems = new List<MenuItem>();
        }

        public Menu(List<MenuItem> menuItems)
        {
            MenuItems = menuItems;
        }
    }
}
