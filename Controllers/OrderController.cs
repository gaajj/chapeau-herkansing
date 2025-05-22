using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Repositories;
using Microsoft.AspNetCore.Authorization;
using ChapeauHerkansing.ViewModels.Ordering;
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

        // hard coded table for now
        public IActionResult Index(string menuType = "", int tableId = 2, string category = "")
        {
            Order order = _orderRepository.GetOrderByTable(tableId);
            Menu? menu = null;
            List<string> categories = _menuRepository.GetAllCategories();

            if (!string.IsNullOrEmpty(menuType))
            {
                menu = _menuRepository.GetFilteredMenu(menuType, category);
                if (menu == null || (menu.MenuItems.Count == 0 && !string.IsNullOrEmpty(category)))
                {
                    menu = new Menu(0, menuType)
                    {
                        MenuItems = new List<MenuItem>()
                    };
                }
            }

            MenuViewModel model = new MenuViewModel
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
        public IActionResult AddMenuItemToOrder(MenuItemAddViewModel model)
        {
            Order order = _orderRepository.GetOrderById(model.OrderId);
            MenuItem menuItem = _menuRepository.GetMenuItemById(model.MenuItemId);
            Staff staff = new Staff(2, "", "", "", "", Role.Waiter); // hard coded for now
            try
            {
                OrderLine? existingLine = null;
                
                foreach (OrderLine line in order.OrderLines)
                {
                    if (line.MenuItem.MenuItemID == model.MenuItemId && string.IsNullOrWhiteSpace(line.Note))
                    {
                        existingLine = line;
                        break;
                    }
                }

                if (existingLine != null)
                {
                    _orderRepository.UpdateOrderLineAmount(existingLine.OrderLineID, existingLine.Amount + model.Amount);
                }
                else
                {
                    _orderRepository.AddMenuItemToOrder(order, menuItem, staff, model.Amount);
                }

                TempData["Message"] = "Menu item successfully added.";
            }
            catch
            {
                TempData["Error"] = "An error occurred while adding the menu item to the order.";
            }

            return RedirectToAction("Index", new { tableId = order.Table.TableID });
        }

        [HttpPost]
        public IActionResult RemoveOrderLine(OrderLineUpdateViewModel model)
        {
            try
            {
                if (model.Amount > 1)
                {
                    _orderRepository.UpdateOrderLineAmount(model.OrderLineId, model.Amount - 1);
                }
                else
                {
                    _orderRepository.RemoveOrderLine(model.OrderLineId);
                }

                TempData["Message"] = "Item removed from order.";
            }
            catch
            {
                TempData["Error"] = "An error occurred while removing the item.";
            }

            return RedirectToAction("Index", new { tableId = model.TableId });
        }

        [HttpPost]
        public IActionResult EditOrderLineNote(OrderLineNoteViewModel model)
        {
            try
            {
                _orderRepository.UpdateOrderLineNote(model.OrderLineId, model.Note);
                TempData["Message"] = "Note updated.";
            }
            catch
            {
                TempData["Error"] = "Could not update the note.";
            }

            return RedirectToAction("Index", new { model.TableId});
        }
    }
}
