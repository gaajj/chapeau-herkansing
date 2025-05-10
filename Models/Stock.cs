namespace ChapeauHerkansing.Models
{
    public class Stock
    {
        public int StockID { get; set; }
        public int? MenuItemID { get; set; }
        public int? Amount { get; set; }      

        // Basic Constructor
        public Stock(int stockID, int? menuItemID, int? amount)
        {
            StockID = stockID;
            MenuItemID = menuItemID;
            Amount = amount;
        }
    }
}

