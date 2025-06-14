using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.ViewModels.Ordering;

namespace ChapeauHerkansing.Services.Interfaces
{
    public interface IOrderService
    {
        MenuViewModel GetOrderView(int tableId, MenuType? menuType, MenuCategory? category);
        Order GetOrderByTable(int tableId);
        Order GetOrderById(int orderId);
        MenuItem GetMenuItemById(int menuItemId);
        void AddOrderLineToOrder(OrderLine line);
        void RemoveOrderLine(int orderLineId, int menuItemId, int amount);
        void UpdateOrderLineNote(int orderLineId, string note);
        void UpdateStock(int menuItemId, int amount);
        void UpdateOrderLineAmount(int orderLineId, int amount);
        void CreateOrderForTable(int tableId);
    }
}
