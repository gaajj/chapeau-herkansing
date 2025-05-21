namespace ChapeauHerkansing.Models
{
    public class Stock
    {
        public int StockID { get; set; }
        public MenuItem MenuItem { get; set; }
        public int? Amount { get; set; }

        public Stock(int stockID, MenuItem menuItem, int? amount)
        {
            StockID = stockID;
            MenuItem = menuItem;
            Amount = amount;
        }
    }
}
