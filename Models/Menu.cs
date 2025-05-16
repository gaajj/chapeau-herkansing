using ChapeauHerkansing.Models.Enums;

namespace ChapeauHerkansing.Models
{
    public class Menu
    {
        public int MenuID { get; set; }
        public MenuType MenuType { get; set; }

        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
}
