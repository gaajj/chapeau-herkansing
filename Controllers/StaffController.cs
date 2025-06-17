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

        public IActionResult Index()
        {
            List<Staff> staffList = _staffService.GetAllStaff();
            return View(staffList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new StaffCreateViewModel());
        }

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

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Staff staff = _staffService.GetStaffById(id);
            if (staff == null)
                return NotFound();

            StaffEditViewModel model = new()
            {
                Id = staff.Id,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                Username = staff.Username,
                Role = staff.Role,
                IsDeleted = staff.IsDeleted
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int id, StaffEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all fields correctly.";
                return View(model);
            }

            try
            {
                Staff existingStaff = _staffService.GetStaffById(id);
                if (existingStaff == null)
                    return NotFound();

                string finalPassword = string.IsNullOrWhiteSpace(model.Password)
                    ? existingStaff.Password
                    : BCrypt.Net.BCrypt.HashPassword(model.Password);

                StaffEditViewModel updatedModel = new()
                {
                    Id = id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Username = model.Username,
                    Password = finalPassword,
                    Role = model.Role,
                    IsDeleted = model.IsDeleted
                };

                _staffService.UpdateStaff(id, updatedModel);
                TempData["Message"] = "Staff member successfully updated.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to update staff member: {ex.Message}";
                return View(model);
            }
        }

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
