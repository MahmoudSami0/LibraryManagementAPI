using BookManagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookManagementSystem.EF.Data.Config
{
    public class GenreConfig : IEntityTypeConfiguration<Genre>
    {
        public void Configure(EntityTypeBuilder<Genre> builder)
        {
            builder.HasKey(g => g.Id);
            builder.Property(g => g.GenreName).HasMaxLength(150).IsRequired();

            builder.HasIndex(g => g.GenreName).IsUnique();
            
            builder.ToTable("Genres");
        }
    }

}
