using ChapeauHerkansing.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChapeauHerkansing.Controllers
{
    public class Bar_KitchenController : Controller
    {
        public IActionResult Index()
        {
       /*     List<Order> orders = new List<Order>
        {
            new Order (1, 1, null, null),
            new Order (2, 2, null , null),
            new Order (3, 3, null, null),
            new Order ( 4, 4, null, null),
            new Order (5, 5, null, null)
        }; */

            return View("~/Views/Bar_Kitchen/Index.cshtml");
        }
    }
}
