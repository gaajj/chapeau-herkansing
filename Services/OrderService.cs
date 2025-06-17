using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Repositories;
using ChapeauHerkansing.Repositories.Interfaces;
using ChapeauHerkansing.Services.Interfaces;
using ChapeauHerkansing.ViewModels.Ordering;
using System.Security.Claims;

namespace ChapeauHerkansing.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly ITableService _tableService;

        public OrderService(IOrderRepository orderRepository, IMenuItemRepository menuItemRepository, ITableService tableService)
        {
            _orderRepository = orderRepository;
            _menuItemRepository = menuItemRepository;
            _tableService = tableService;
        }

        public OrderMenuViewModel GetOrderView(int tableId, MenuType? menuType, MenuCategory? category)
        {
            try
            {
                Order order = GetOrderByTable(tableId);

                // If no order exists, make one and mark table as occupied
                if (order == null)
                {
                    Table? table = _tableService.GetTableById(tableId);
                    if (table != null)
                    {
                        CreateOrderForTable(tableId);
                        _tableService.UpdateTableStatus(tableId, TableStatus.Occupied);
                        order = GetOrderByTable(tableId);
                    }
                }

                Menu menu = null;

                // If menu is selected (Lunch,Dinner,Drinks) get menu items by the menu type
                if (menuType.HasValue)
                {
                    menu = _menuItemRepository.GetMenuItemsByMenuType(menuType.Value);

                    // If no items were found for the category in this menu type, return empty menu (to show empty state in view)
                    if ((menu == null || menu.MenuItems.Count == 0) && category != null)
                    {
                        menu = new Menu();
                    }
                    else if (category != null)
                    {
                        menu.MenuItems = menu.MenuItems
                            .Where(item => item.Category == category.Value)
                            .ToList();
                    }
                }

                return new OrderMenuViewModel
                {
                    Order = order,
                    Menu = menu,
                    SelectedCategory = category,
                    MenuType = menuType
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to build the order view.", ex);
            }
        }

        public Order GetOrderByTable(int tableId)
        {
            return _orderRepository.GetOrderByTable(tableId);
        }

        public Order GetOrderById(int orderId)
        {
            return _orderRepository.GetOrderById(orderId);
        }

        public void AddOrderLineToOrder(OrderLine line)
        {
            if (line.MenuItem.StockAmount < line.Amount)
                throw new InvalidOperationException("Not enough stock available to add this item.");

            // Check if same item with same note already exists (make new orderline if orderline with note exists)
            OrderLine existingLine = line.Order.OrderLines
                .FirstOrDefault(l => l.MenuItem.MenuItemID == line.MenuItem.MenuItemID && l.Note == line.Note);

            if (existingLine != null)
            {
                // If found, increase quantity
                int newAmount = existingLine.Amount + line.Amount;
                _orderRepository.UpdateOrderLineAmount(existingLine.OrderLineID, newAmount);
            }
            else
            {
                // Else, add new orderLine
                _orderRepository.AddMenuItemToOrder(line.Order, line.MenuItem, line.Staff, line.Amount, line.OrderStatus);
            }

            _menuItemRepository.UpdateStock(line.MenuItem.MenuItemID, -line.Amount);
        }

        public void RemoveOrderLine(OrderLineUpdateViewModel model)
        {
            if (model.Amount <= 0)
            {
                throw new ArgumentException("Invalid item amount.");
            }
            if (model.RemoveAll || model.Amount == 1)
            {
                _orderRepository.RemoveOrderLine(model.OrderLineId);
                _menuItemRepository.UpdateStock(model.MenuItemId, model.Amount);
            }
            else
            {
                _orderRepository.UpdateOrderLineAmount(model.OrderLineId, model.Amount - 1);
                _menuItemRepository.UpdateStock(model.MenuItemId, 1);
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

        public void CreateOrderForTable(int tableId)
        {
            _orderRepository.CreateOrderForTable(tableId);
        }
    }
}
