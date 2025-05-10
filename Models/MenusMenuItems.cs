namespace ChapeauHerkansing.Models
{
    public class MenusMenuItems
    {
        public int MenuID { get; set; }
        public int MenuItemID { get; set; }

        // Basic Constructor
        public MenusMenuItems(int menuID, int menuItemID)
        {
            MenuID = menuID;
            MenuItemID = menuItemID;
        }
    }
}
