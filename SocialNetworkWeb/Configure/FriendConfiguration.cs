using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetworkWeb.Models;

namespace SocialNetworkWeb.Configure
{
    public class FriendConfiguration : IEntityTypeConfiguration <Friend>
    {
        public void Configure (EntityTypeBuilder <Friend> builder)
        {
            builder.ToTable("Friends").HasKey(p => p.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
        }
    }
}
