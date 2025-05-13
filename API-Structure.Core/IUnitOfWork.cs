using API_Structure.Core.Models;
using API_Structure.Core.Repositories;
using BookManagementSystem.Core.Models;

namespace API_Structure.Core;

public interface IUnitOfWork : IDisposable
{
    public IUserRepository Users { get; }
    public IRoleRepository Roles { get;  }
    public IBookRepository Books { get; }
    public IBaseRepository<UserRoles> UserRoles { get; }
    public IBaseRepository<Author> Authors { get; }
    public IBaseRepository<Genre> Genres { get; }
    public IBaseRepository<Favorite> Favorites { get; }
    public IBaseRepository<UserBooks> UserBooks { get; }
    public IBaseRepository<Review> Reviews { get; }
    public IBaseRepository<BookGenres> BookGenres { get; }
    Task SaveAsync();
}