using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Repositories;
using ChapeauHerkansing.ViewModels.Ordering;

namespace ChapeauHerkansing.Services
{
    public class OrderService
    {
        private readonly OrderRepository _orderRepository;
        private readonly MenuItemRepository _menuItemRepository;

        public OrderService(OrderRepository orderRepository, MenuItemRepository menuItemRepository)
        {
            _orderRepository = orderRepository;
            _menuItemRepository = menuItemRepository;
        }

        public Order GetOrderByTable(int tableId)
        {
            return _orderRepository.GetOrderByTable(tableId);
        }

        public Order GetOrderById(int orderId)
        {
            return _orderRepository.GetOrderById(orderId);
        }

        public MenuItem GetMenuItemById(int menuItemId)
        {
            return _menuItemRepository.GetMenuItemById(menuItemId);
        }

        public void AddMenuItemToOrder(Order order, MenuItem menuItem, Staff staff, MenuItemAddViewModel model)
        {
            OrderLine? existingLine = order.OrderLines
                .FirstOrDefault(line => line.MenuItem.MenuItemID == menuItem.MenuItemID && line.Note == model.Note);

            if (existingLine != null)
            {
                _orderRepository.UpdateOrderLineAmount(existingLine.OrderLineID, existingLine.Amount + model.Amount);
            }
            else
            {
                _orderRepository.AddMenuItemToOrder(order, menuItem, staff, model.Amount, OrderStatus.Ordered);
            }

            _menuItemRepository.UpdateStock(menuItem.MenuItemID, -model.Amount);
        }

        public void RemoveOrderLine(int orderLineId, int menuItemId, int amount)
        {
            if (amount > 1)
            {
                _orderRepository.UpdateOrderLineAmount(orderLineId, amount - 1);
                _menuItemRepository.UpdateStock(menuItemId, 1);
            }
            else
            {
                _orderRepository.RemoveOrderLine(orderLineId);
                _menuItemRepository.UpdateStock(menuItemId, amount);
            }
        }



        public void UpdateOrderLineNote(int orderLineId, string note)
        {
            _orderRepository.UpdateOrderLineNote(orderLineId, note);
        }

        public void UpdateStock(int menuItemId, int amount)
        {
            _menuItemRepository.UpdateStock(menuItemId, amount);
        }

        public void UpdateOrderLineAmount(int orderLineId, int amount)
        {
            _orderRepository.UpdateOrderLineAmount(orderLineId, amount);
        }
    }
}
