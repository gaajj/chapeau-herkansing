namespace ChapeauHerkansing.ViewModels.Ordering
{
    public class OrderLineUpdateViewModel
    {
        public int OrderLineId { get; set; }
        public int Amount { get; set; }
        public int TableId { get; set; }
        public int MenuItemId { get; set; }
        public bool RemoveAll { get; set; }
    }
}
