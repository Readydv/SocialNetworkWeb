using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetworkWeb.Data.Repository;
using SocialNetworkWeb.Data.UoW;
using SocialNetworkWeb.Extensions;
using SocialNetworkWeb.Models;
using SocialNetworkWeb.ViewModels.Account;

namespace SocialNetworkWeb.Controllers
{
    [Authorize]
    public class AccountManagerController : Controller
    {
        private IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountManagerController> _logger;

        public AccountManagerController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, IUnitOfWork unitOfWork, ILogger<AccountManagerController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View("Home/Login");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [AllowAnonymous]
        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl); // можно RedirectToAction("MyPage")
                        }
                        else
                        {
                            return RedirectToAction("MyPage", "AccountManager");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь с таким email не найден");
                }
                return View(model); // чтобы отобразить ошибки
            }
            return RedirectToAction("Index", "Home");
        }


        [Route("Edit")]
        [HttpGet]
        public IActionResult Edit()
        {
            var user = User;

            var result = _userManager.GetUserAsync(user);

            var editModel = _mapper.Map<UserEditViewModel>(result.Result);

            return View("~/Views/Shared/Edit.cshtml", editModel);
        }

        [Authorize]
        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> Update(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);

                if (user == null)
                {
                    return NotFound($"User with ID '{model.UserId}' not found.");
                }

                user.Convert(model);

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("MyPage", "AccountManager"); // Перенаправьте на страницу профиля
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("~/Views/Shared/Edit.cshtml", model);
                }
            }
            ModelState.AddModelError("", "Некорректные данные");
            return View("~/Views/Shared/Edit.cshtml", model);
        }

        [Authorize]
        [Route("MyPage")]
        [HttpGet]
        public async Task <IActionResult> MyPage()
        {
            var user = User;

            var result = await _userManager.GetUserAsync(user);

            var model = new UserViewModel(result);

            model.Friends = await GetAllFriend(model.User);

            return View("~/Views/Shared/UserProfile.cshtml", model);
        }

        private async Task <List <User>> GetAllFriend(User user)
        {
            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            return repository.GetFriendsByUser(user);
        }

        [Route("UserList")]
        [HttpGet]
        public async Task<IActionResult> UserList(string search)
        {
            var model = await CreateSearch(search);
            return View("~/Views/Shared/UserList.cshtml", model);
        }

        private async Task<SearchViewModel> CreateSearch(string search)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);

            List<User> list; // Объявляем list вне if/else

            if (string.IsNullOrEmpty(search))
            {
                // Если строка поиска пуста, загружаем всех пользователей
                list = _userManager.Users.ToList();
            }
            else
            {
                // Если строка поиска не пуста, фильтруем пользователей
                list = _userManager.Users
                   .Where(x => (x.FirstName + " " + x.LastName).ToLower().Contains(search.ToLower()))
                    .ToList();
            }

            var withfriend = await GetAllFriend();

            var data = new List<UserWithFriendExt>();
            list.ForEach(x =>
            {
                var t = _mapper.Map<UserWithFriendExt>(x);
                t.IsFriendWithCurrent = withfriend.Any(y => y.Id == x.Id || x.Id == result.Id);
                data.Add(t);
            });

            var model = new SearchViewModel()
            {
                UserList = data
            };

            return model;
        }


        private async Task <List<User>> GetAllFriend()
        {
            var user = User;

            var result = await _userManager.GetUserAsync(user);

            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            return repository.GetFriendsByUser(result);
        }

        public async Task <IActionResult> AddFriend(string id)
        {
            var currentUser = User;

            var result = await _userManager.GetUserAsync(currentUser);

            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Friend> () as FriendsRepository;

            repository.AddFriend(result, friend);

            return RedirectToAction("MyPage", "AccountManager");
        }

        public async Task<IActionResult> DeleteFriend (string id)
        {
            var currentUser = User;

            var result = await _userManager.GetUserAsync(currentUser);

            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            repository.DeleteFriend(result, friend);

            return RedirectToAction("MyPage", "AccountManager");
        }

        [Route("Chat")]
        [HttpPost]
        public async Task<IActionResult> Chat(string id)
        {
            var currentUser = User;

            var result = await _userManager.GetUserAsync(currentUser);
            var friend = await _userManager.FindByIdAsync(id);

            var repo = _unitOfWork.GetRepository<Message>() as MessageRepository;

            var mess = repo.GetMessages(result, friend);

            var model = new ChatViewModel()
            {
                You = result,
                ToWhom = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
            };

            return View("~/Views/Shared/Chat.cshtml", model);
        }

        [Route("NewMessage/{id}")]
        [HttpPost]
        public async Task<IActionResult> NewMessage(string id, ChatViewModel model)
        {
            var result = await _userManager.GetUserAsync(User);
            if (result == null)
                return BadRequest("Пользователь не найден");

            var friend = await _userManager.FindByIdAsync(id);
            if (friend == null)
                return BadRequest("Получатель не найден");

            if (model.NewMessage == null || string.IsNullOrWhiteSpace(model.NewMessage.Text))
                return BadRequest("Сообщение пустое");

            var repo = _unitOfWork.GetRepository<Message>() as MessageRepository;

            var item = new Message()
            {
                SenderId = result.Id,
                RecipientId = friend.Id,
                Text = model.NewMessage.Text
            };

            repo.Create(item);

            var mess = repo.GetMessages(result, friend);

            var total = new ChatViewModel()
            {
                You = result,
                ToWhom = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
                NewMessage = new MessageViewModel()
            };

            return View("~/Views/Shared/Chat.cshtml", total);
        }

        private async Task<ChatViewModel> GenerateChat(string id)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);
            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Message>() as MessageRepository;

            var mess = repository.GetMessages(result, friend);

            var model = new ChatViewModel()
            {
                You = result,
                ToWhom = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
            };

            return model;
        }

        [Route("Chat")]
        [HttpGet]
        public async Task<IActionResult> Chat()
        {

            var id = Request.Query["id"];

            var model = await GenerateChat(id);
            return View("Chat", model);

        }

        [Route("Logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}