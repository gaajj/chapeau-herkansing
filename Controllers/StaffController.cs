using Microsoft.AspNetCore.Mvc;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Services;
using ChapeauHerkansing.ViewModels.Management;

namespace ChapeauHerkansing.Controllers
{
    public class StaffController : Controller
    {
        private readonly StaffService _staffService;

        public StaffController(StaffService staffService)
        {
            _staffService = staffService;
        }

        public IActionResult Index()
        {
            // Haalt alle medewerkers op (ook gedeactiveerden als true)
            StaffCollection staffCollection = _staffService.GetAllStaff(true);
            return View(staffCollection);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(StaffCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all fields correctly.";
                return View(model);
            }

            // Hashing gebeurt binnen de service
            _staffService.AddStaff(model);
            TempData["Message"] = "Staff member successfully added.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int staffId)
        {
            // Haalt de medewerker op die aangepast moet worden
            Staff staff = _staffService.GetStaffById(staffId);
            if (staff == null)
            {
                return NotFound();
            }

            // Zet bestaande data in een edit viewmodel
            StaffEditViewModel model = new StaffEditViewModel
            {
                Id = staff.Id,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                Username = staff.Username,
                Password = staff.Password,
                Role = staff.Role,
                IsDeleted = staff.IsDeleted
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int staffId, StaffEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all fields correctly.";
                return View(model);
            }

            Staff existingStaff = _staffService.GetStaffById(staffId);

            // Als er geen nieuw wachtwoord is ingevoerd, houd het oude wachtwoord
            StaffEditViewModel updatedModel = new StaffEditViewModel
            {
                Id = staffId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Username = model.Username,
                Password = string.IsNullOrWhiteSpace(model.Password) ? existingStaff.Password : model.Password,
                Role = model.Role,
                IsDeleted = model.IsDeleted
            };

            _staffService.UpdateStaff(staffId, updatedModel);
            TempData["Message"] = "Staff member successfully updated.";
            return RedirectToAction("Index");
        }

        public IActionResult ToggleActive(int id)
        {
            try
            {
                // Activeer of deactiveer medewerker
                bool isNowInactive = _staffService.ToggleStaffActive(id);
                TempData["Message"] = isNowInactive
                    ? "Staff member deactivated."
                    : "Staff member reactivated.";
            }
            catch
            {
                TempData["Error"] = "Failed to update staff status.";
            }

            return RedirectToAction("Index");
        }
    }
}
