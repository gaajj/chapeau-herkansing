using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Repositories;
using ChapeauHerkansing.Services.Interfaces;
using ChapeauHerkansing.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChapeauHerkansing.Services;
using ChapeauHerkansing.ViewModels.Bar_Kitchen;
using System.Security.Claims;
using System.Data;
using static NuGet.Packaging.PackagingConstants;
using System.Collections.Generic;

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
            Role role = GetUserRole();
           
            OrdersViewModel ordersViewModel = BuildOrderViewModel(false);

            return View("~/Views/Bar_Kitchen/Index.cshtml", ordersViewModel);
        }

        public IActionResult GetOngoingOrders()
        {
            OrdersViewModel ordersViewModel = BuildOrderViewModel(false);
            return PartialView("_OrdersPartial", ordersViewModel);
        }
        [HttpPost]
        public IActionResult ToggleStatus([FromBody] int orderlineid)
        {
           _bar_KitchenService.ToggleOrderLineStatus(orderlineid);
            return Ok();
        }



        public IActionResult FinishedOrders()
        {
            OrdersViewModel ordersViewModel = BuildOrderViewModel(true);

            return View("~/Views/Bar_Kitchen/Index.cshtml", ordersViewModel);
        }
        public IActionResult GetFinishedOrders()
        {  OrdersViewModel ordersViewModel = BuildOrderViewModel(true);
            return PartialView("_OrdersPartial", ordersViewModel);
        }

        

        private Role GetUserRole()
        {
            return Enum.Parse<Role>(User.FindFirstValue(ClaimTypes.Role));
        }

        private OrdersViewModel BuildOrderViewModel(bool isFinished)
        {
            Role role = GetUserRole();
            List<Order> orders = isFinished
                ? _bar_KitchenService.GetFinishedOrders(role)
                : _bar_KitchenService.GetOngoingOrders(role);

            return new OrdersViewModel
            {
                Orders = orders,
                Role = role,
                isFinished = isFinished 
            };
        }

    }
}

