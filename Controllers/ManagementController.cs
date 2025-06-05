using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Services;
using ChapeauHerkansing.ViewModels.Management;

namespace ChapeauHerkansing.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagementController : Controller
    {
        private readonly FinancialService _financialService;
        private readonly MenuService _menuService;

        public ManagementController(MenuService menuService, FinancialService financialService)
        {
            _menuService = menuService;
            _financialService = financialService;
        }

        public IActionResult Index(MenuType menuType = MenuType.Lunch, MenuCategory? category = null)
        {
            Menu menu = _menuService.GetFilteredMenu(menuType, category, includeDeleted: true);

            MenuManagementViewModel viewModel = new MenuManagementViewModel
            {
                Menu = menu,
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

        [HttpGet]
        public IActionResult Edit(int id)
        {
            MenuItem item = _menuService.GetMenuItemById(id);
            if (item == null)
            {
                return NotFound();
            }

            MenuItemCreateViewModel model = new MenuItemCreateViewModel
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
            catch
            {
                TempData["Error"] = "Something went wrong while updating the menu item.";
                return View(model);
            }
        }

        public IActionResult ToggleActive(int id)
        {
            try
            {
                bool toggled = _menuService.ToggleMenuItemActive(id);
                TempData["Message"] = toggled ? "Menu item deactivated." : "Menu item reactivated.";
            }
            catch
            {
                TempData["Error"] = "Failed to change the item's active status.";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Financial(DateTime? startDate, DateTime? endDate, string period = "month")
        {
            DateTime start;
            DateTime end;

            if (period == "month")
            {
                start = DateTime.Now.AddMonths(-1);
                end = DateTime.Now;
            }
            else if (period == "quarter")
            {
                start = DateTime.Now.AddMonths(-3);
                end = DateTime.Now;
            }
            else if (period == "year")
            {
                start = DateTime.Now.AddYears(-1);
                end = DateTime.Now;
            }
            else if (period == "custom" && startDate.HasValue && endDate.HasValue)
            {
                start = startDate.Value;
                end = endDate.Value;
            }
            else
            {
                // terugval naar de laatste maand als er geen geldige periode is gekozen
                start = DateTime.Now.AddMonths(-1);
                end = DateTime.Now;
            }

            List<FinancialData> data = _financialService.GetFinancialOverview(start, end);
            decimal tips = _financialService.GetTotalTips(start, end);

            FinancialOverviewViewModel model = new FinancialOverviewViewModel
            {
                SelectedPeriod = period,
                StartDate = start,
                EndDate = end,
                TotalSalesByType = new Dictionary<string, int>(),
                TotalIncomeByType = new Dictionary<string, decimal>(),
                TotalTipAmount = tips
            };

            foreach (FinancialData item in data)
            {
                model.TotalSalesByType[item.MenuType] = item.TotalSales;
                model.TotalIncomeByType[item.MenuType] = item.TotalIncome;
            }

            return View(model);
        }


    }
}