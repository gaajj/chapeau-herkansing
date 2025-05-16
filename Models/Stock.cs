namespace ChapeauHerkansing.Models
{
    public class Stock
    {
        public int StockID { get; set; }
        public int? Amount { get; set; }

        // Optioneel — als je MenuItem wilt koppelen
        public MenuItem MenuItem { get; set; }
    }
}
