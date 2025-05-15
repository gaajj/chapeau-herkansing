using Microsoft.AspNetCore.Mvc;
using ChapeauHerkansing.Models;

namespace ChapeauHerkansing.Controllers
{
    public class StaffController : Controller
    {
        private readonly IRepository _userRepository;
        private List<Staff> staff;
        public StaffController(IRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            List<Staff> staff = _userRepository.GetAll();
            return View(staff);
        }
    }
}
