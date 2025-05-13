using BookManagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookManagementSystem.EF.Data.Config
{
    public class BookConfig : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Title).HasMaxLength(256).IsRequired();
            builder.Property(b => b.Description).HasMaxLength(1000).IsRequired();
            builder.Property(b => b.PublishYear).IsRequired();

            builder.HasOne(a => a.Author)
                .WithMany(b => b.Books)
                .HasForeignKey(a => a.AuthorId);

            builder.HasIndex(b => b.Title).IsUnique();
            builder.HasIndex(b => b.PublishYear);

            builder.ToTable("Books");
        }
    }

}
