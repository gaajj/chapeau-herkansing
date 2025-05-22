using ChapeauHerkansing.Models;

namespace ChapeauHerkansing.ViewModels.Ordering
{
    public class MenuViewModel
    {
        public Order Order { get; set; }
        public Menu Menu { get; set; }
        public List<string> Categories { get; set; } = new();
        public string SelectedCategory { get; set; } = "";
        public string MenuType { get; set; } = "";
    }
}
