using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ChapeauHerkansing.Controllers
{
    public class OrderController : Controller
    {
        private readonly MenuRepository _menuRepository;

        public OrderController(MenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public IActionResult Index(string menuType = "drinks")
        {
            Menu menu = _menuRepository.GetFilteredMenu(menuType);
            ViewData["MenuType"] = menuType;
            return View(menu);
        }

        // Change menu type without reloading page
        [HttpGet]
        public IActionResult ChangeMenuType(string menuType)
        {
            Menu? menu = _menuRepository.GetFilteredMenu(menuType);
            if (menu == null)
            {
                return NotFound();
            }

            ViewData["MenuType"] = menuType;
            return PartialView("_MenuPartial", menu);
        }
    }
}
