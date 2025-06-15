using ChapeauHerkansing.Models;

namespace ChapeauHerkansing.ViewModels.Ordering
{
    public class OrderLineAddViewModel
    {
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public int Amount { get; set; }
        public string? Note { get; set; }
        public int TableId { get; set; }
    }
}