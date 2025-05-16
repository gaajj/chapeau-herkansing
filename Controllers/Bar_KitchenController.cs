using ChapeauHerkansing.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChapeauHerkansing.Controllers
{
    public class Bar_KitchenController : Controller
    {
      private readonly IRepository<Order> _orderRepository;

        public Bar_KitchenController(IRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public IActionResult Index()
        {
            List<Order> orders = _orderRepository.GetAll();

            return View("~/Views/Bar_Kitchen/Index.cshtml", orders);
        }
    }
}
