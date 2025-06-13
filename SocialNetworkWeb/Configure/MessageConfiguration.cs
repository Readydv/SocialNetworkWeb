using Microsoft.EntityFrameworkCore;
using SocialNetworkWeb.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialNetworkWeb.Configure
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder <Message> builder)
        {
            builder.ToTable("Message").HasKey(p =>  p.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
        }
    }
}
