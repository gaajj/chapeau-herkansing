using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ChapeauHerkansing.Controllers
{
    public class Bar_KitchenController : Controller
    {
        private readonly OrderRepository _orderRepository;

        public Bar_KitchenController(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public IActionResult Index()
        {
            List<Order> orders = _orderRepository.GetAll();

            return View("~/Views/Bar_Kitchen/Index.cshtml", orders);
        }

        public IActionResult GetOngoingOrders()
        {
            List<Order> orders = _orderRepository.GetAllNotReady(); 
            return PartialView("_OrdersPartial", orders);
        }
        [HttpPost]
        public IActionResult ToggleStatus([FromBody]int orderlineid)
        {
                _orderRepository.ToggleOrderLineStatus(orderlineid);
            return Ok();
        }

        public IActionResult FinishedOrders()
        {
            

            List<Order> orders = _orderRepository.GetAllReady();

            return View("FinishedOrders", orders); 
        }
        public IActionResult GetFinishedOrders()
        {
            List<Order> orders = _orderRepository.GetAllReady();
            return PartialView("_OrdersPartial", orders);
        }


    }
    }

