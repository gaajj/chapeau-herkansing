using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Repositories;
using ChapeauHerkansing.ViewModels.Ordering;
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
            Order order = _orderRepository.GetOrderByTable(tableId);
            Menu? menu = null;
            List<string> categories = _menuRepository.GetAllCategories();

            if (!string.IsNullOrEmpty(menuType))
            {
                menu = _menuRepository.GetFilteredMenu(menuType, category);
            }

            var model = new MenuViewModel
            {
                Order = order,
                Menu = menu,
                Categories = categories,
                SelectedCategory = category,
                MenuType = menuType     
            };

            ViewData["Title"] = $"Order of table #{model.Order.Table.TableID}";
            return View(model);
        }

        [HttpPost]
        public IActionResult AddMenuItemToOrder(int orderId, int menuItemId, int amount)
        {
            Order order = _orderRepository.GetOrderById(orderId);
            MenuItem menuItem = _menuRepository.GetMenuItemById(menuItemId);
            Staff staff = new Staff(2, "", "", "", "", Role.Waiter);
            try
            {
                OrderLine? existingLine = null;
                
                foreach (OrderLine line in order.OrderLines)
                {
                    if (line.MenuItem.MenuItemID == menuItemId)
                    {
                        existingLine = line;
                        break;
                    }
                }

                if (existingLine != null)
                {
                    _orderRepository.UpdateOrderLineAmount(existingLine.OrderLineID, existingLine.Amount + amount);
                }
                else
                {
                    _orderRepository.AddMenuItemToOrder(order, menuItem, staff, amount);
                }

                TempData["Message"] = "Menu item successfully added.";
            }
            catch
            {
                TempData["Error"] = "An error occurred while adding the menu item to the order.";
            }

            return RedirectToAction("Index", new { tableId = order.Table.TableID });
        }
    }
}
