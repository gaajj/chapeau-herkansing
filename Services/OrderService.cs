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

        public MenuViewModel GetOrderView(int tableId, MenuType? menuType, MenuCategory? category)
        {
            try
            {
                Order order = GetOrderByTable(tableId);

                if (order == null)
                {
                    Table? table = _tableService.GetTableById(tableId);
                    if (table != null && (table.Status == TableStatus.Free || table.Status == TableStatus.Reserved))
                    {
                        CreateOrderForTable(tableId);
                        _tableService.UpdateTableStatus(tableId, TableStatus.Occupied);
                        order = GetOrderByTable(tableId);
                    }
                }

                Menu menu = null;

                if (menuType.HasValue)
                {
                    menu = _menuItemRepository.GetMenuItemsByMenuType(menuType.Value);

                    if ((menu == null || menu.MenuItems.Count == 0) && category != null)
                    {
                        menu = new Menu(); // show empty state
                    }
                    else if (category != null)
                    {
                        menu.MenuItems = menu.MenuItems
                            .Where(item => item.Category == category.Value)
                            .ToList();
                    }
                }

                return new MenuViewModel
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

        public MenuItem GetMenuItemById(int menuItemId)
        {
            return _menuItemRepository.GetMenuItemById(menuItemId);
        }

        public void AddOrderLineToOrder(OrderLine line)
        {
            if (line.MenuItem.StockAmount < line.Amount)
                throw new InvalidOperationException("Not enough stock available to add this item.");

            OrderLine existingLine = line.Order.OrderLines
                .FirstOrDefault(l => l.MenuItem.MenuItemID == line.MenuItem.MenuItemID && l.Note == line.Note);

            if (existingLine != null)
            {
                int newAmount = existingLine.Amount + line.Amount;
                _orderRepository.UpdateOrderLineAmount(existingLine.OrderLineID, newAmount);
            }
            else
            {
                _orderRepository.AddMenuItemToOrder(line.Order, line.MenuItem, line.Staff, line.Amount, line.OrderStatus);
            }

            _menuItemRepository.UpdateStock(line.MenuItem.MenuItemID, -line.Amount);
        }

        public void RemoveOrderLine(int orderLineId, int menuItemId, int amount, bool removeAll)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Invalid item amount.");
            }
            if (removeAll || amount == 1)
            {
                _orderRepository.RemoveOrderLine(orderLineId);
                _menuItemRepository.UpdateStock(menuItemId, amount);
            }
            else
            {
                _orderRepository.UpdateOrderLineAmount(orderLineId, amount - 1);
                _menuItemRepository.UpdateStock(menuItemId, 1);
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

        public void CreateOrderForTable(int tableId)
        {
            _orderRepository.CreateOrderForTable(tableId);
        }
    }
}
