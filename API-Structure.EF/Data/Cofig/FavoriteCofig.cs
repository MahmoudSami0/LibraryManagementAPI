using BookManagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagementSystem.EF.Data.Config
{
    public class FavoriteCofig : IEntityTypeConfiguration<Favorite>
    {
        public void Configure(EntityTypeBuilder<Favorite> builder)
        {
            builder.HasKey(f => f.Id);

            builder.HasOne(u => u.User)
                .WithMany(f => f.Favorites)
                .HasForeignKey(u => u.UserId);
            
            builder.HasOne(b => b.Book)
                .WithMany(f => f.Favorites)
                .HasForeignKey(b => b.BookId);

            builder.ToTable("UserFavorites");
        }
    }
}
