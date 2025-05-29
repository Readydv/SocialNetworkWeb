using SocialNetworkWeb.ViewModels.Account;

namespace SocialNetworkWeb.ViewModels.Account
{
    public class MainViewModel
    {
        public LoginViewModel LoginView { get; set; }
        public RegisterViewModel RegisterView { get; set; }

        public MainViewModel()
        {
            RegisterView = new RegisterViewModel();
            LoginView = new LoginViewModel();
        }
    }
}
