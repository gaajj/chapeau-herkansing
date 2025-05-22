using ChapeauHerkansing.Repositories;

namespace ChapeauHerkansing.Services
{
    public class StockService
    {
        private readonly StockRepository _stockRepository;

        public StockService(StockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public int CreateInitialStock(int amount)
        {
            return _stockRepository.InsertStock(amount);
        }

        public void UpdateStock(int menuItemId, int newAmount)
        {
            _stockRepository.UpdateStock(menuItemId, newAmount);
        }

        public int? GetStockForMenuItem(int menuItemId)
        {
            return _stockRepository.GetStockAmount(menuItemId);
        }
    }

}
