namespace ChapeauHerkansing.Models
{
    public class FinancialData
    {
        public string MenuType { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalIncome { get; set; }

        public FinancialData(string menuType, int totalSales, decimal totalIncome)
        {
            MenuType = menuType;
            TotalSales = totalSales;
            TotalIncome = totalIncome;
        }

        public FinancialData() { }
    }
}
