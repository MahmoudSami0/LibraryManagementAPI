using API_Structure.Core.Models;
using BookManagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Structure.EF.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRoles> UserRoles { get; set; }
    public DbSet<BookGenres> BookGenres { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    //public DbSet<Favorite> Favorites { get; set; }
    //public DbSet<Review> Reviews { get; set; }
    //public DbSet<UserBooks> UserBooks { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}