using System.Collections.Generic;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;

namespace ChapeauHerkansing.ViewModels.Management
{
    public class MenuManagementViewModel
    {
        public Menu Menu { get; set; } = new();
        public MenuType SelectedMenuType { get; set; }
        public MenuCategory? SelectedCategory { get; set; }

        public List<MenuType> MenuTypes { get; set; } = new();
        public List<MenuCategory> Categories { get; set; } = new();
    }
}
