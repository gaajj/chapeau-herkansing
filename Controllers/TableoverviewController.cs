using Microsoft.AspNetCore.Mvc;

namespace ChapeauHerkansing.Controllers
{
    public class TableOverviewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
