using Microsoft.AspNetCore.Mvc;

namespace ChapeauHerkansing.Controllers
{
    public class UsersController : Controller
    {
        private readonly IRepository _userRepository;

        public UsersController(IRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            var users = _userRepository.GetAll();
            return View(users);
        }
    }
}
