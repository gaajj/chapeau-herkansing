using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChapeauHerkansing.Controllers
{
    [Authorize(Roles = "Waiter")]
    public class OrderController : Controller
    {
        private readonly MenuRepository _menuRepository;
        private readonly OrderRepository _orderRepository;

        public OrderController(MenuRepository menuRepository, OrderRepository orderRepository)
        {
            _menuRepository = menuRepository;
            _orderRepository = orderRepository;
        }

        public IActionResult Index(string menuType = "", int tableId = 2)
        {
            if (!string.IsNullOrEmpty(menuType))
            {
                Menu? menu = _menuRepository.GetFilteredMenu(menuType);
                ViewData["MenuType"] = menuType;
                ViewData["FilteredMenu"] = menu;
                return View(_orderRepository.GetOrderByTable(tableId));
            }

            Order? order = _orderRepository.GetOrderByTable(tableId);
            return View(order);
        }
    }
}
