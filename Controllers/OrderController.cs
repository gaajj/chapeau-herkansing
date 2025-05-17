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

        public IActionResult Index(string menuType = "", int tableId = 2)
        {
            if (!string.IsNullOrEmpty(menuType))
            {
                Menu? menu = _menuRepository.GetFilteredMenu(menuType);
                ViewData["MenuType"] = menuType;
                return View("MenuView", menu);
            }

            Order? order = _orderRepository.GetOrderByTable(tableId);
            return View("OrderView", order);
        }

        [HttpGet]
        public IActionResult GetOrder()
        {
            Order? order = _orderRepository.GetOrderByTable(2);
            return View(order);

        }

        // Change menu type without reloading page
        [HttpGet]
        public IActionResult ChangeMenuType(string menuType)
        {
            Menu? menu = _menuRepository.GetFilteredMenu(menuType);
            if (menu == null)
            {
                return NotFound();
            }
            return PartialView("_MenuPartial", menu);
        }
    }
}
