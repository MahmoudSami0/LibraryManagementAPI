using BookManagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace BookManagementSystem.EF.Data.Config
{
    public class AuthorConfig : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a =>  a.AuthorName).HasMaxLength(256).IsRequired();

            builder.HasIndex(a => a.AuthorName).IsUnique();

            builder.ToTable("Authors");
        }
    }

}
