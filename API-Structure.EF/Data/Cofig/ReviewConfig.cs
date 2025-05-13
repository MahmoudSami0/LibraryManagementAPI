using BookManagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookManagementSystem.EF.Data.Config
{
    public class ReviewConfig : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.ReviewContent).IsRequired();

            builder.HasOne(u => u.User)
                .WithMany(r => r.Reviews)
                .HasForeignKey(u => u.UserId);

            builder.HasOne(b => b.Book)
                .WithMany(r => r.Reviews)
                .HasForeignKey(b => b.BookId);

            builder.ToTable("Reviews");
        }
    }

}
