using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Repositories;
using ChapeauHerkansing.Services.Interfaces;
using ChapeauHerkansing.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChapeauHerkansing.Services;

namespace ChapeauHerkansing.Controllers
{
    [Authorize(Roles = "Barman,Chef")]
    public class Bar_KitchenController : Controller
    {
        private readonly IBar_KitchenService _bar_KitchenService;

        public Bar_KitchenController(IBar_KitchenService bar_KitchenService)
        {
            _bar_KitchenService = bar_KitchenService;
        }

        public IActionResult Index()
        {
            List<Order> orders = _bar_KitchenService.GetOngoingOrders();
            // views op een andere manier ophalen
            // view model
            return View("~/Views/Bar_Kitchen/Index.cshtml", orders);
        }

        public IActionResult GetOngoingOrders()
        {
            List<Order> orders = _bar_KitchenService.GetOngoingOrders();
            return PartialView("_OrdersPartial", orders);
        }
        [HttpPost]
        public IActionResult ToggleStatus([FromBody] int orderlineid)
        {
           _bar_KitchenService.ToggleOrderLineStatus(orderlineid);
            return Ok();
        }

        public IActionResult FinishedOrders()
        {


            List<Order> orders = _bar_KitchenService.GetFinishedOrders();

            return View("FinishedOrders", orders);
        }
        public IActionResult GetFinishedOrders()
        {
            List<Order> orders = _bar_KitchenService.GetFinishedOrders();
            return PartialView("_OrdersPartial", orders);
        }


    }
}

