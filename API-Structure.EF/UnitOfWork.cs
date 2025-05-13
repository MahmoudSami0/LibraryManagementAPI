using API_Structure.Core;
using API_Structure.Core.Models;
using API_Structure.Core.Repositories;
using API_Structure.EF.Data;
using API_Structure.EF.Repositories;
using BookManagementSystem.Core.Models;

namespace API_Structure.EF;

public class UnitOfWork : IUnitOfWork,IAsyncDisposable
{
    private readonly ApplicationDbContext _context;
    public IUserRepository Users { get; private set; }
    public IRoleRepository Roles { get; private set; }
    public IBookRepository Books { get; private set; }
    public IBaseRepository<UserRoles> UserRoles { get; private set; }
    public IBaseRepository<Author> Authors { get; private set; }
    public IBaseRepository<Genre> Genres { get; private set; }
    public IBaseRepository<Favorite> Favorites { get; private set; }
    public IBaseRepository<UserBooks> UserBooks { get; private set; }
    public IBaseRepository<Review> Reviews { get; private set; }
    public IBaseRepository<BookGenres> BookGenres { get; private set; }


    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        Roles = new RoleRepository(_context);
        Books = new BookRepository(_context);
        UserRoles = new BaseRepository<UserRoles>(_context);
        Authors = new BaseRepository<Author>(_context);
        Genres = new BaseRepository<Genre>(_context);
        Favorites = new BaseRepository<Favorite>(_context);
        UserBooks = new BaseRepository<UserBooks>(_context);
        Reviews = new BaseRepository<Review>(_context);
        BookGenres = new BaseRepository<BookGenres>(_context);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}