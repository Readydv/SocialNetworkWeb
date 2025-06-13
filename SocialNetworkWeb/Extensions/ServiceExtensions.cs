using SocialNetworkWeb.Data.Repository;
using SocialNetworkWeb.Data.UoW;

namespace SocialNetworkWeb.Extensions
{
    public static class ServiceExtentions
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IServiceCollection AddCustomRepository<TEntity, IRepository>(this IServiceCollection services)
                 where TEntity : class
                 where IRepository : class, IRepository<TEntity>
        {
            services.AddScoped(typeof(IRepository<TEntity>), typeof(IRepository));

            return services;
        }

    }
}
