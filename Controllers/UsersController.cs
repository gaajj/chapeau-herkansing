using Microsoft.AspNetCore.Mvc;

namespace ChapeauHerkansing.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUsersRepository _userRepository;

        public UsersController(IUsersRepository userRepository)
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
