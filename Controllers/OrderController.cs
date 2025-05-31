using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ChapeauHerkansing.Controllers
{
    public class OrderController : Controller
    {
        private readonly MenuItemRepository _menuItemRepository;
        private readonly OrderRepository _orderRepository;

        public OrderController(MenuItemRepository menuItemRepository, OrderRepository orderRepository)
        {
            _menuItemRepository = menuItemRepository;
            _orderRepository = orderRepository;
        }

        public IActionResult Index(string menuType = "", int tableId = 2)
        {
            if (!string.IsNullOrEmpty(menuType))
            {
                if (Enum.TryParse<MenuType>(menuType, true, out MenuType parsedType))
                {
                    List<MenuItem> items = _menuItemRepository.GetMenuItemsByMenuType(parsedType);
                    ViewData["MenuType"] = parsedType;
                    ViewData["FilteredMenuItems"] = items;
                }
            }

            Order? order = _orderRepository.GetOrderByTable(tableId);
            return View(order);
        }
    }
}
