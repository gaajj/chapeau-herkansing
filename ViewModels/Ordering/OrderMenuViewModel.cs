using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;

namespace ChapeauHerkansing.ViewModels.Ordering
{
    public class OrderMenuViewModel
    {
        public Order Order { get; set; }
        public Menu Menu { get; set; }
        public MenuCategory? SelectedCategory { get; set; }
        public MenuType? MenuType { get; set; }
    }
}
