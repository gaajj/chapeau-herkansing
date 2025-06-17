using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Services;
using ChapeauHerkansing.ViewModels.Management;
using ChapeauHerkansing.Services.Interfaces;

namespace ChapeauHerkansing.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagementController : Controller
    {
        private readonly FinancialService _financialService;
        private readonly IMenuService _menuService;

        public ManagementController(IMenuService menuService, FinancialService financialService)
        {
            _menuService = menuService;
            _financialService = financialService;
        }

        // Laat het beheeroverzicht van menu-items zien
        public IActionResult Index(MenuType menuType = MenuType.Lunch, MenuCategory? category = null)
        {
            Menu menu = _menuService.GetFilteredMenu(menuType, category);

            MenuManagementViewModel viewModel = new()
            {
                Menu = menu,
                SelectedMenuType = menuType,
                SelectedCategory = category,
                MenuTypes = Enum.GetValues(typeof(MenuType)).Cast<MenuType>().ToList(),
                Categories = Enum.GetValues(typeof(MenuCategory)).Cast<MenuCategory>().ToList()
            };

            return View(viewModel);
        }

        // Toont het formulier om een nieuw menu-item aan te maken
        [HttpGet]
        public IActionResult Create()
        {
            return View(new MenuItemCreateViewModel());
        }

        // Verwerkt het formulier om een nieuw menu-item toe te voegen
        [HttpPost]
        public IActionResult Create(MenuItemCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "The form is invalid. Please check all fields.";
                return View(model);
            }


            try
            {
                _menuService.AddMenuItem(model);
                TempData["Message"] = "Menu item successfully added.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error while adding item: {ex.Message}";
                return View(model);
            }
        }


        // Toont het formulier om een bestaand menu-item te bewerken
        [HttpGet]
        public IActionResult Edit(int id)
        {
            MenuItem item = _menuService.GetMenuItemById(id);
            if (item == null)
                return NotFound();

            MenuItemCreateViewModel model = new()
            {
                Name = item.Name,
                Price = item.Price,
                Category = item.Category,
                IsAlcoholic = item.IsAlcoholic,
                StockAmount = item.StockAmount,
                MenuType = item.MenuType
            };

            return View(model);
        }

        // Verwerkt het bewerken van een bestaand menu-item
        [HttpPost]
        public IActionResult Edit(int id, MenuItemCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "The form is invalid. Please check your input.";
                return View(model);
            }

            try
            {
                _menuService.UpdateMenuItem(id, model);
                TempData["Message"] = "Menu item successfully updated.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Something went wrong while updating the menu item: {ex.Message}";
                return View(model);
            }
        }

        // Zet een menu-item aan of uit (soft delete)
        public IActionResult ToggleActive(int id)
        {
            try
            {
                bool toggled = _menuService.ToggleMenuItemActive(id);
                TempData["Message"] = toggled ? "Menu item deactivated." : "Menu item reactivated.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to change the item's active stateus: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // Toont het financiële overzicht op basis van periode of aangepaste datums
        public IActionResult Financial(DateTime? startDate, DateTime? endDate, string period = "month")
        {
            if (period == "custom")
            {
                if (!startDate.HasValue || !endDate.HasValue)
                {
                    TempData["Error"] = "Please select both a start and end date for the custom period.";
                    return RedirectToAction("Financial");
                }

                if (startDate > endDate)
                {
                    TempData["Error"] = "Start date cannot be after end date.";
                    return RedirectToAction("Financial");
                }
            }

            (DateTime start, DateTime end) = GetStartEndDateByPeriod(period, startDate, endDate);

            try
            {
                List<FinancialData> data = _financialService.GetFinancialOverview(start, end);

                FinancialOverviewViewModel model = new()
                {
                    SelectedPeriod = period,
                    StartDate = start,
                    EndDate = end,
                    ReportItems = data
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading financial data: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Bepaalt begin- en einddatum op basis van geselecteerde periode
        private (DateTime Start, DateTime End) GetStartEndDateByPeriod(string period, DateTime? startDate, DateTime? endDate)
        {
            DateTime now = DateTime.Now;

            return period switch
            {
                "month" => (now.AddMonths(-1), now),
                "quarter" => (now.AddMonths(-3), now),
                "year" => (now.AddYears(-1), now),
                "custom" when startDate.HasValue && endDate.HasValue => (startDate.Value, endDate.Value),
                _ => (now.AddMonths(-1), now)
            };
        }
    }
}
