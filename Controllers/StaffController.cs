using Microsoft.AspNetCore.Mvc;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories;
using System.Collections.Generic;

namespace ChapeauHerkansing.Controllers
{
    public class StaffController : Controller
    {
        private readonly IStaffRepository _staffRepository;

        public StaffController(IStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }

        public IActionResult Index()
        {
            List<Staff> staffList = _staffRepository.GetAllStaff();
            return View(staffList);
        }
    }
}
