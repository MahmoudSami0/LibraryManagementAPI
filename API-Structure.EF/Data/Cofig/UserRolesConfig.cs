using API_Structure.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_Structure.EF.Data.Cofig
{
    public class UserRolesConfig : IEntityTypeConfiguration<UserRoles>
    {
        public void Configure(EntityTypeBuilder<UserRoles> builder)
        { 
            builder.HasKey(ur => new {ur.UserId, ur.RoleId});

            builder.HasOne(u => u.User)
                .WithMany(ur => ur.UserRoles);

            builder.HasOne(r => r.Role)
                .WithMany(ur => ur.UserRoles);



            builder.ToTable("UserRoles");
        }
    }
}
