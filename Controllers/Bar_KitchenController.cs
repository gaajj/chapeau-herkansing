using ChapeauHerkansing.Models;
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

        public IActionResult GetOrders()
        {
            var orders = _orderRepository.GetAll(); 
            return PartialView("_OrdersPartial", orders);
        }
        [HttpPost]
        public IActionResult ToggleStatus([FromBody]int orderlineid)
        {
                _orderRepository.ToggleOrderLineStatus(orderlineid);
            return Ok();
        }
          

        }
    }

