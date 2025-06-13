using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialNetworkWeb.Configure;
using SocialNetworkWeb.Models;

namespace SocialNetworkWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated(); // В реальном проекте лучше использовать миграции
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration<Friend>(new FriendConfiguration());
            builder.ApplyConfiguration<Message>(new MessageConfiguration());
        }
    }
}
