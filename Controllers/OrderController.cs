using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Services.Interfaces;
using ChapeauHerkansing.ViewModels.Ordering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChapeauHerkansing.Controllers
{
    [Authorize(Roles = "Waiter")]
    public class OrderController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IOrderService _orderService;
        private readonly IStaffService _staffService;

        public OrderController(IMenuService menuService, IOrderService orderService, IStaffService staffService)
        {
            _menuService = menuService;
            _orderService = orderService;
            _staffService = staffService;
        }

        public IActionResult Index(int tableId = 2, MenuType? menuType = null, MenuCategory? category = null)
        {
            try
            {
                OrderMenuViewModel viewModel = _orderService.GetOrderView(tableId, menuType, category);
                return View(viewModel);
            }
            catch
            {
                TempData["Error"] = "Failed to load the order view.";
                return RedirectToAction("Index", new { tableId });
            }
        }

        [HttpPost]
        public IActionResult AddMenuItemToOrder(OrderLineAddViewModel model)
        {
            try
            {
                Order order = _orderService.GetOrderById(model.OrderId);
                MenuItem menuItem = _menuService.GetMenuItemById(model.MenuItemId);
                int staffId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                Staff staff = _staffService.GetStaffById(staffId);

                OrderLine orderLine = new OrderLine(0, order, menuItem, staff, model.Amount, DateTime.Now, model.Note, OrderStatus.Ordered);
                _orderService.AddOrderLineToOrder(orderLine);

                TempData["Message"] = "Item successfully added.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch
            {
                TempData["Error"] = "Failed to add menu item to the order.";
            }

            return RedirectToAction("Index", new { tableId = model.TableId });
        }

        [HttpPost]
        public IActionResult RemoveOrderLine(OrderLineUpdateViewModel model)
        {
            try
            {
                _orderService.RemoveOrderLine(model.OrderLineId, model.MenuItemId, model.Amount, model.RemoveAll);
                TempData["Message"] = model.RemoveAll ? "Items removed from order." : "Item removed from order.";
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

        [HttpPost]
        public IActionResult PayOrder(int orderId, int tableId)
        {
            try
            {
                return RedirectToAction("Create", "Payment", new { orderId });
            }
            catch
            {
                TempData["Error"] = "Could not redirect to payment.";
                return RedirectToAction("Index", new { tableId });
            }
        }
    }
}
