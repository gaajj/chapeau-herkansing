using Microsoft.AspNetCore.Mvc;
using ChapeauHerkansing.Models;

namespace ChapeauHerkansing.Controllers
{
    public class StaffController : Controller
    {
        private readonly IRepository<Staff> _userRepository;
      
        public StaffController(IRepository<Staff> userRepository)
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
