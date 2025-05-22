namespace ChapeauHerkansing.ViewModels.Ordering
{
    public class OrderLineNoteViewModel
    {
        public int OrderLineId { get; set; }
        public int TableId { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
