using Microsoft.AspNetCore.Mvc;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Services;
using ChapeauHerkansing.ViewModels.Management;
using ChapeauHerkansing.Services.Interfaces;

namespace ChapeauHerkansing.Controllers
{
    public class StaffController : Controller
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        // Laat alle medewerkers zien (ook gedeactiveerde)
        public IActionResult Index()
        {
            List<Staff> staffList = _staffService.GetAllStaff(includeDeleted: true);
            return View(staffList);
        }

        // Toont het formulier om een nieuwe medewerker toe te voegen
        [HttpGet]
        public IActionResult Create()
        {
            return View(new StaffCreateViewModel());
        }

        // Verwerkt het formulier en voegt de medewerker toe
        [HttpPost]
        public IActionResult Create(StaffCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all fields correctly.";
                return View(model);
            }

            if (_staffService.UsernameExists(model.Username))
            {
                TempData["Error"] = "This username is already taken.";
                return View(model);
            }

            try
            {
                _staffService.AddStaff(model);
                TempData["Message"] = "Staff member successfully added.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to add staff member: {ex.Message}";
                return View(model);
            }
        }

        // Toont het formulier om een medewerker te bewerken (zonder wachtwoord)
        [HttpGet]
        public IActionResult Edit(int staffId)
        {
            Staff staff = _staffService.GetStaffById(staffId);
            if (staff == null)
                return NotFound();

            StaffEditViewModel model = new()
            {
                Id = staff.Id,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                Username = staff.Username,
                // Password wordt niet meegegeven!
                Role = staff.Role,
                IsDeleted = staff.IsDeleted
            };

            return View(model);
        }

        // Verwerkt de bewerking van een medewerker
        [HttpPost]
        public IActionResult Edit(int staffId, StaffEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all fields correctly.";
                return View(model);
            }

            try
            {
                Staff existingStaff = _staffService.GetStaffById(staffId);
                if (existingStaff == null)
                    return NotFound();

                // Gebruik het bestaande wachtwoord als er geen nieuw is opgegeven
                string finalPassword;

                if (string.IsNullOrWhiteSpace(model.Password))
                {
                    // Geen nieuw wachtwoord ingevuld = gebruik het oude
                    finalPassword = existingStaff.Password;
                }
                else
                {
                    // Nieuw wachtwoord ingevuld = gebruik het nieuwe
                    finalPassword = model.Password;
                }


                StaffEditViewModel updatedModel = new()
                {
                    Id = staffId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Username = model.Username,
                    Password = finalPassword,
                    Role = model.Role,
                    IsDeleted = model.IsDeleted
                };

                _staffService.UpdateStaff(staffId, updatedModel);
                TempData["Message"] = "Staff member successfully updated.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to update staff member: {ex.Message}";
                return View(model);
            }
        }

        // Zet medewerker aan of uit (soft delete)
        public IActionResult ToggleActive(int id)
        {
            try
            {
                bool isNowInactive = _staffService.ToggleStaffActive(id);
                TempData["Message"] = isNowInactive
                    ? "Staff member deactivated."
                    : "Staff member reactivated.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to update staff status: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
