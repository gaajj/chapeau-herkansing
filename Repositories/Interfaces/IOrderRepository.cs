using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;

namespace ChapeauHerkansing.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Order GetOrderById(int orderId);
        List<Order> GetAllOrders();
       
        Order GetOrderByTable(int tableId);
     
        void UpdateOrderLineAmount(int orderLineId, int newAmount);
        void AddMenuItemToOrder(Order order, MenuItem menuItem, Staff staff, int amount, OrderStatus orderStatus);
        void RemoveOrderLine(int orderLineId);
        void UpdateOrderLineNote(int orderLineId, string note);
        void CreateOrderForTable(int tableId);
        void SoftDeleteOrder(int orderId);
    }
}
