using BookManagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookManagementSystem.EF.Data.Config
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.FirstName).HasMaxLength(50).IsRequired();
            builder.Property(u => u.LastName).HasMaxLength(256).IsRequired();
            builder.Property(u => u.Email).HasMaxLength(150).IsRequired();
            builder.Property(u => u.UserName).HasMaxLength(50).IsRequired();
            builder.Property(u => u.Password).IsRequired();
            builder.Property(u => u.Phone).HasMaxLength(20);

            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => u.UserName).IsUnique();
            builder.HasIndex(u => u.Phone).IsUnique();
            builder.HasIndex(u => new {u.FirstName, u.LastName});

            
            builder.ToTable("Users");
        }
    }

}
