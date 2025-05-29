using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetworkWeb.Models;
using SocialNetworkWeb.ViewModels.Account;

namespace SocialNetworkWeb.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        public UserController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();  
            }
            var viewModel = new UserViewModel(user);
            return View("~/Views/Shared/UserProfile.cshtml", viewModel);
        }

        public IActionResult Edit()
        {
            return View();
        }
    }
}
