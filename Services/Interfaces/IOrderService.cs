using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.ViewModels.Ordering;

namespace ChapeauHerkansing.Services.Interfaces
{
    public interface IOrderService
    {
        OrderMenuViewModel GetOrderView(int tableId, MenuType? menuType, MenuCategory? category);
        Order GetOrderByTable(int tableId);
        Order GetOrderById(int orderId);
        void AddOrderLineToOrder(OrderLine line);
        void RemoveOrderLine(int orderLineId, int menuItemId, int amount, bool removeAll);
        void UpdateOrderLineNote(int orderLineId, string note);
        void UpdateStock(int menuItemId, int amount);
        void CreateOrderForTable(int tableId);
    }
}
