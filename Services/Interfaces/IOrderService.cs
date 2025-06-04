using ChapeauHerkansing.Models;
using ChapeauHerkansing.ViewModels.Ordering;

namespace ChapeauHerkansing.Services.Interfaces
{
    public interface IOrderService
    {
        Order GetOrderByTable(int tableId);
        Order GetOrderById(int orderId);
        MenuItem GetMenuItemById(int menuItemId);
        void AddMenuItemToOrder(Order order, MenuItem menuItem, Staff staff, MenuItemAddViewModel model);
        void RemoveOrderLine(int orderLineId, int menuItemId, int amount);
        void UpdateOrderLineNote(int orderLineId, string note);
        void UpdateStock(int menuItemId, int amount);
        void UpdateOrderLineAmount(int orderLineId, int amount);
        void CreateOrderForTable(int tableId);
    }
}
