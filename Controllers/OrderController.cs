using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Repositories;
using ChapeauHerkansing.Services;
using Microsoft.AspNetCore.Authorization;
using ChapeauHerkansing.ViewModels.Ordering;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ChapeauHerkansing.Controllers
{
    [Authorize(Roles = "Waiter")]
    public class OrderController : Controller
    {
        private readonly MenuService _menuService;
        private readonly OrderService _orderService;
        private readonly TableService _tableService;

        public OrderController(MenuService menuService, OrderService orderService, TableService tableService)
        {
            _menuService = menuService;
            _orderService = orderService;
            _tableService = tableService;
        }

        public IActionResult Index(int tableId = 2, MenuType? menuType = null, MenuCategory? category = null)
        {
            Order order = _orderService.GetOrderByTable(tableId);

            if (order == null)
            {
                Table? table = _tableService.GetTableById(tableId);
                if (table != null && table.Status == TableStatus.Free)
                {
                    _orderService.CreateOrderForTable(tableId);
                    _tableService.UpdateTableStatus(tableId, TableStatus.Occupied);

                    order = _orderService.GetOrderByTable(tableId);
                }
            }

            Menu? menu = null;

            if (menuType.HasValue)
            {
                menu = _menuService.GetMenuItemsByMenuType(menuType.Value);

                if (menu == null || (menu.MenuItems.Count == 0 && category != null))
                {
                    menu = new Menu(); // forces no items message instead of going back to index
                }
                else if (category != null)
                {
                    menu.MenuItems = menu.MenuItems
                        .Where(item => item.Category == category.Value)
                        .ToList();
                }
            }

            MenuViewModel model = new MenuViewModel
            {
                Order = order,
                Menu = menu,
                SelectedCategory = category,
                MenuType = menuType
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AddMenuItemToOrder(MenuItemAddViewModel model)
        {
            Order order = _orderService.GetOrderById(model.OrderId);
            MenuItem menuItem = _menuService.GetMenuItemById(model.MenuItemId);
            Staff staff = new Staff(10, "", "", "", "", Role.Waiter); // hard coded for now

            try
            {
                if (menuItem.StockAmount < model.Amount)
                {
                    TempData["Error"] = "Not enough stock available to add this item.";
                    return RedirectToAction("Index", new { tableId = order.Table.TableID });
                }

                _orderService.AddMenuItemToOrder(order, menuItem, staff, model);
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
                _orderService.RemoveOrderLine(model.OrderLineId, model.MenuItemId, model.Amount);
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
                _orderService.UpdateOrderLineNote(model.OrderLineId, model.Note);
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
