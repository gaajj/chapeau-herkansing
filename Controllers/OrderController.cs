using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Repositories;
using ChapeauHerkansing.ViewModels.Ordering;
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

        // hard coded table for now
        public IActionResult Index(int tableId = 2, MenuType? menuType = null, MenuCategory? category = null)
        {
            Order order = _orderRepository.GetOrderByTable(tableId);
            Menu? menu = null;

            if (menuType.HasValue)
            {
                menu = _menuItemRepository.GetMenuItemsByMenuType(menuType.Value);

                if (menu == null || (menu.MenuItems.Count == 0 && category != null))
                {
                    menu = new Menu(); // forces no items message instead of going back to index
                }
                else
                {
                    if (category != null)
                    {
                        menu.MenuItems = menu.MenuItems
                            .Where(item => item.Category == category.Value)
                            .OrderBy(item => item.Name)
                            .ToList();
                    }
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
            Order order = _orderRepository.GetOrderById(model.OrderId);
            MenuItem menuItem = _menuItemRepository.GetMenuItemById(model.MenuItemId);
            Staff staff = new Staff(10, "", "", "", "", Role.Waiter); // hard coded for now
            try
            {
                if (menuItem.StockAmount < model.Amount)
                {
                    TempData["Error"] = "Not enough stock available to add this item.";
                    return RedirectToAction("Index", new { tableId = order.Table.TableID });
                }
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
                    _orderRepository.AddMenuItemToOrder(order, menuItem, staff, model.Amount, OrderStatus.Ordered);
                }

                _menuItemRepository.UpdateStock(model.MenuItemId, -model.Amount);
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
                    _menuItemRepository.UpdateStock(model.MenuItemId, 1);
                }
                else
                {
                    _orderRepository.RemoveOrderLine(model.OrderLineId);
                    _menuItemRepository.UpdateStock(model.MenuItemId, model.Amount);
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
