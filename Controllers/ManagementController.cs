using Microsoft.AspNetCore.Mvc;
using ChapeauHerkansing.Services;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.ViewModels.Management;
using Microsoft.AspNetCore.Authorization;

namespace ChapeauHerkansing.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagementController : Controller
    {
        private readonly MenuService _menuService;

        public ManagementController(MenuService menuService)
        {
            _menuService = menuService;
        }

        public IActionResult Index(MenuType menuType = MenuType.Lunch, MenuCategory? category = null)
        {
            // var mag niet
            var menuItems = _menuService.GetFilteredMenuItems(menuType, category, includeDeleted: true);

            var viewModel = new MenuManagementViewModel
            {
                MenuItems = menuItems,
                SelectedMenuType = menuType,
                SelectedCategory = category,
                MenuTypes = Enum.GetValues(typeof(MenuType)).Cast<MenuType>().ToList(),
                Categories = Enum.GetValues(typeof(MenuCategory)).Cast<MenuCategory>().ToList()
            };

            return View(viewModel);
        }




        [HttpGet]
        public IActionResult Create()
        {
            return View(new MenuItemCreateViewModel());
        }


        [HttpPost]
        public IActionResult Create(MenuItemCreateViewModel model)
        {
            if (!ModelState.IsValid)
            { // alle teksten in het engels
                TempData["Error"] = "Formulier is ongeldig. Controleer alle velden.";
                return View(model);
            }

            try
            {
                _menuService.AddMenuItem(model);
                TempData["Message"] = "Gerecht succesvol toegevoegd.";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Error"] = "Er is iets misgegaan bij het toevoegen van het gerecht.";
                return View(model);
            }
        }



        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _menuService.GetMenuItemById(id);
            if (item == null)
                return NotFound();

            var model = new MenuItemCreateViewModel
            {
                Name = item.Name,
                Price = item.Price,
                Category = item.Category,
                IsAlcoholic = item.IsAlcoholic,
                StockAmount = item.StockAmount ?? 0,
                MenuType = item.MenuType
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int id, MenuItemCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Formulier is ongeldig. Controleer de invoer.";
                return View(model);
            }

            try
            {
                _menuService.UpdateMenuItem(id, model);
                TempData["Message"] = "Gerecht succesvol bijgewerkt.";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Error"] = "Er ging iets mis bij het bijwerken van het gerecht.";
                return View(model);
            }
        }



        public IActionResult ToggleActive(int id)
        {
            try
            {
                bool toggled = _menuService.ToggleMenuItemActive(id);
                TempData["Message"] = toggled ? "Gerecht is gedeactiveerd." : "Gerecht is opnieuw geactiveerd.";
            }
            catch
            {
                TempData["Error"] = "Er ging iets mis bij het aanpassen van de status.";
            }

            return RedirectToAction("Index");
        }




    }
}
