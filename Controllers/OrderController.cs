using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ChapeauHerkansing.Controllers
{
    public class OrderController : Controller
    {
        private readonly MenuRepository _menuRepository;
        private readonly OrderRepository _orderRepository;

        public OrderController(MenuRepository menuRepository, OrderRepository orderRepository)
        {
            _menuRepository = menuRepository;
            _orderRepository = orderRepository;
        }

        public IActionResult Index(string menuType = "", int tableId = 2, string category = "")
        {
            Order order = _orderRepository.GetOrderByTable(tableId) ?? new Order(0, new Table(tableId, null, null, null), false);
            Menu? menu = null;
            List<string> categories = _menuRepository.GetAllCategories();

            if (!string.IsNullOrEmpty(menuType))
            {
                menu = _menuRepository.GetFilteredMenu(menuType, category);
                ViewData["MenuType"] = menuType;
                ViewData["Category"] = category;

                if (menu == null || (menu.MenuItems.Count == 0 && !string.IsNullOrEmpty(category)))
                {
                    // Force empty menu warning
                    menu = new Menu(0, menuType);
                    ViewData["Menu"] = menu;
                }
            }

            ViewData["Menu"] = menu;
            ViewData["Categories"] = categories;
            ViewData["Title"] = $"Order of Table #{tableId}";

            return View(order);
        }
    }
}
