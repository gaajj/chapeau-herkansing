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
            Order? order = _orderRepository.GetOrderByTable(tableId);

            if (!string.IsNullOrEmpty(menuType))
            {
                Menu? menu = _menuRepository.GetFilteredMenu(menuType, category);
                ViewData["MenuType"] = menuType;
                ViewData["Menu"] = menu;
                ViewData["Category"] = category;
                ViewData["Categories"] = _menuRepository.GetAllCategories();

                if (menu == null || (menu.MenuItems.Count == 0 && !string.IsNullOrEmpty(category)))
                {
                    // Force empty menu warning
                    menu = new Menu(0, menuType);
                    ViewData["Menu"] = menu;
                }
            }
            else
            {
                ViewData["Menu"] = null;
                ViewData["Categories"] = null;
            }

            ViewData["Title"] = $"Order of Table #{tableId}";
            return View(order);
        }
    }
}
