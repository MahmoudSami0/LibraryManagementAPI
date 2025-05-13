using BookManagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace BookManagementSystem.EF.Data.Config
{
    public class UserBooksConfig : IEntityTypeConfiguration<UserBooks>
    {
        public void Configure(EntityTypeBuilder<UserBooks> builder)
        {
            builder.HasKey(ub => ub.Id);

            builder.HasOne(u => u.User)
                .WithMany(ub => ub.UserBooks)
                .HasForeignKey(u => u.UserId);

            builder.HasOne(b => b.Book)
                .WithMany(ub => ub.UserBooks)
                .HasForeignKey(b => b.BookId);

            builder.ToTable("UserBooks");
        }
    }
}
