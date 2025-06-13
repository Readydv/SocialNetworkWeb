using System.Diagnostics;
using SocialNetworkWeb.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using SocialNetworkWeb.Models;
using SocialNetworkWeb.ViewModels.Account;
using Microsoft.AspNetCore.Identity;

namespace SocialNetworkWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<User> _signInManager;

        public HomeController(ILogger<HomeController> logger, SignInManager<User> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
        }

        [Route("")]
        [Route("[controller]/[action]")]
        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("MyPage", "AccountManager");
            }
            else
            {
                var model = new MainViewModel
                {
                    LoginView = new LoginViewModel(),
                    RegisterView = new RegisterViewModel()
                };
                return View(model);
            }
        }

        [Route("[action]")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
