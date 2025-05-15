namespace ChapeauHerkansing.Models
{
    public class Menu
    {
        public int MenuID { get; set; }
        public string Type { get; set; }

        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

        // Basic Constructor
        public Menu(int menuID, string type)
        {
            MenuID = menuID;
            Type = type;
        }
    }
}

