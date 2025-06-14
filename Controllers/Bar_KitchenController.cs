using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Repositories;
using ChapeauHerkansing.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChapeauHerkansing.Controllers
{
    [Authorize(Roles = "Barman,Chef")]
    public class Bar_KitchenController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public Bar_KitchenController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public IActionResult Index()
        {
            List<Order> orders = _orderRepository.GetAllOrdersByStatus(OrderStatus.Ordered);
            // views op een andere manier ophalen
            // view model
            return View("~/Views/Bar_Kitchen/Index.cshtml", orders);
        }

        public IActionResult GetOngoingOrders()
        {
            List<Order> orders = _orderRepository.GetAllOrdersByStatus(OrderStatus.Ordered);
            return PartialView("_OrdersPartial", orders);
        }
        [HttpPost]
        public IActionResult ToggleStatus([FromBody] int orderlineid)
        {
            _orderRepository.ToggleOrderLineStatus(orderlineid);
            return Ok();
        }

        public IActionResult FinishedOrders()
        {


            List<Order> orders = _orderRepository.GetAllOrdersByStatus(OrderStatus.Ready);

            return View("FinishedOrders", orders);
        }
        public IActionResult GetFinishedOrders()
        {
            List<Order> orders = _orderRepository.GetAllOrdersByStatus(OrderStatus.Ready);
            return PartialView("_OrdersPartial", orders);
        }


    }
}

