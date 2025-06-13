using AutoMapper;
using SocialNetworkWeb.Models;
using SocialNetworkWeb.ViewModels.Account;

namespace SocialNetworkWeb
{
    public class AutoMapperProfile : Profile    
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterViewModel, User>()
                .ForMember(x => x.BirthDate, opt => opt.MapFrom(c => new DateTime((int)c.Year, (int)c.Month, (int)c.Date)))
                .ForMember(x => x.Email, opt => opt.MapFrom(c => c.EmailReg))
                .ForMember(x => x.UserName, opt => opt.MapFrom(c => c.Login))
                .ForMember(x => x.MiddleName, opt => opt.MapFrom(c => c.Login)); // 👈 Добавлено!
            CreateMap<LoginViewModel, User>();

            CreateMap<UserEditViewModel, User>();
            CreateMap<User, UserEditViewModel>().ForMember(x => x.UserId, opt => opt.MapFrom(c => c.Id));

            CreateMap<UserWithFriendExt, User>();
            CreateMap<User, UserWithFriendExt>();
        }
    }
}
