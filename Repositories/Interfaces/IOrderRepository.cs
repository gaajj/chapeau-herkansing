using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;

namespace ChapeauHerkansing.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Order GetOrderById(int orderId);
        List<Order> GetAllOrders();
        List<Order> GetAllOrdersByStatusAndCategory( OrderStatus orderStatus, Role role);
        Order GetOrderByTable(int tableId);
        void AddMenuItemToOrder(Order order, MenuItem menuItem, Staff staff, int amount, OrderStatus status);
        void ToggleOrderLineStatus(int orderLineId);
        void UpdateOrderLineAmount(int orderLineId, int newAmount);
        void RemoveOrderLine(int orderLineId);
        void UpdateOrderLineNote(int orderLineId, string note);
        void CreateOrderForTable(int tableId);
        void SoftDeleteOrder(int orderId);
    }
}
