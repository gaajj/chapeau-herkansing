using System.Collections.Generic;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;

namespace ChapeauHerkansing.ViewModels.Management
{
    public class MenuManagementViewModel
    {
        public List<MenuItem> MenuItems { get; set; }

        public MenuType SelectedMenuType { get; set; }
        public MenuCategory? SelectedCategory { get; set; }

        public List<MenuType> MenuTypes { get; set; }
        public List<MenuCategory> Categories { get; set; }
    }
}
