using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
            StaffCollection model = _staffService.GetAllStaff(true);
            return View(model);

        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(StaffCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vul alle velden correct in.";
                return View(model);
            }

            _staffService.AddStaff(model);
            TempData["Message"] = "Medewerker succesvol toegevoegd.";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var staff = _staffService.GetStaffById(id);
            if (staff == null)
            {
                return NotFound();
            }

            var model = new StaffEditViewModel
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
        public IActionResult Edit(int id, StaffEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vul alle velden correct in.";
                return View(model);
            }

            var existing = _staffService.GetStaffById(id);

            var updatedModel = new StaffEditViewModel
            {
                Id = id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Username = model.Username,
                // Alleen aanpassen als nieuw wachtwoord is ingevoerd
                Password = string.IsNullOrWhiteSpace(model.Password) ? existing.Password : model.Password,
                Role = model.Role,
                IsDeleted = model.IsDeleted
            };

            _staffService.UpdateStaff(id, updatedModel);
            TempData["Message"] = "Medewerker succesvol bijgewerkt.";
            return RedirectToAction("Index");
        }

        public IActionResult ToggleActive(int id)
        {
            try
            {
                bool isNowInactive = _staffService.ToggleStaffActive(id);
                TempData["Message"] = isNowInactive ? "Medewerker gedeactiveerd." : "Medewerker opnieuw geactiveerd.";
            }
            catch
            {
                TempData["Error"] = "Status aanpassen mislukt.";
            }

            return RedirectToAction("Index");
        }
    }
}
