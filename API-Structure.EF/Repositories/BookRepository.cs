using API_Structure.Core;
using API_Structure.Core.DTOs.BookDtos;
using API_Structure.Core.DTOs.CustomDtos;
using API_Structure.Core.DTOs.GenereDto;
using API_Structure.Core.Helpers;
using API_Structure.Core.Repositories;
using API_Structure.EF.Data;
using BookManagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;


namespace API_Structure.EF.Repositories
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<CustomBookDto?>> GetBooksByGenreAsync(string genreName)
        {
            var books = await CustomFindListAsync<BookGenres, CustomBookDto>(
                predicate: bg => bg.Genre.GenreName.ToLower() == genreName.ToLower() && !bg.IsDeleted,
                selector: bg => new CustomBookDto
                {
                    id = bg.BookId,
                    title = bg.Book.Title,
                    description = bg.Book.Description,
                    authorId = bg.Book.AuthorId,
                    author = bg.Book.Author.AuthorName,
                    publishYear = bg.Book.PublishYear,
                    genres = bg.Book.BookGenres.Select(bg => new GenreDto { genreId = bg.GenreId, GenreName = bg.Genre.GenreName }).ToList(),
                    cover = bg.Book.Cover,
                });

            return books;
        }

        public async Task<string> AddToGenreAsync(AddBookToGenreDto dto)
        {
            try
            {
                var book = await FindAsync(b => b.Title.ToLower() == dto.bookTitle.ToLower());
                var genre = await _context.Genres.SingleOrDefaultAsync(g => g.GenreName.ToLower() == dto.genreName.ToLower());
                

                // Handel Book
                if (book is null || book.IsDeleted)
                    return "Book Not Found!";

                // Handel Genre
                if (genre is null)
                {
                    var newGenre = new Genre { GenreName = dto.genreName };
                    await _context.Genres.AddAsync(newGenre);

                    var newAddBookGenre = new BookGenres()
                    {
                        BookId = book.Id,
                        GenreId = newGenre.Id
                    };

                    await _context.BookGenres.AddAsync(newAddBookGenre);
                    await _context.SaveChangesAsync();

                    return "Book Added To Genre Successfully";
                }

                if (genre.IsDeleted)
                {
                    genre.IsDeleted = false;
                }

                var bookGenre = await _context.BookGenres.SingleOrDefaultAsync(bg => bg.GenreId == genre!.Id && bg.BookId == book.Id);

                // Handel BookGenre
                if (bookGenre is null)
                {
                    var newAddBookGenre = new BookGenres()
                    {
                        BookId = book.Id,
                        GenreId = genre.Id
                    };

                    await _context.BookGenres.AddAsync(newAddBookGenre);
                    await _context.SaveChangesAsync();

                    return "Book Added To Genre Successfully";
                }

                if (await ExistsInGenreAsync(dto))
                {
                    if (bookGenre.IsDeleted)
                    {
                        bookGenre.IsDeleted = false;
                        await _context.SaveChangesAsync();
                        return "Book Added To Genre Successfully";
                    }
                    return "Book Already Assigned To This Genre";
                }

                var newBookGenre = new BookGenres()
                {
                    BookId = book.Id,
                    GenreId = genre.Id
                };

                await _context.BookGenres.AddAsync(newBookGenre);
                await _context.SaveChangesAsync();

                return "Book Added To Genre Successfully";
            }
            catch (Exception e)
            {
                return $"Something Went Wrong Add Book Not Added To Genre \n {e.Message}";
            }
        }

        public async Task<bool> ExistsInGenreAsync(AddBookToGenreDto dto)
        {
            try
            {
                Book? book = await FindAsync(b => b.Title.ToLower() == dto.bookTitle.ToLower());
                Genre? genre = await _context.Genres.SingleOrDefaultAsync(g => g.GenreName.ToLower() == dto.genreName.ToLower());

                if (book is null || genre is null || book.IsDeleted || genre.IsDeleted)
                    return false;

                return await _context.BookGenres.AnyAsync(b => b.BookId == book.Id && b.GenreId == genre.Id);
            }
            catch (Exception e)
            {
                throw new Exception($"Something Went Wrong While Search If Book Exist In Genre \n {e.Message}");
            }
        }

        public async Task<string> AddNewBook(BookDto dto)
        {
            try
            {
                 string allowedExtensions = ".png, .jpg, .jpeg";
                 long allowedSize = 12582912;


                var author = await _context.Authors.SingleOrDefaultAsync(a => a.AuthorName.ToLower() == dto.AuthorName.ToLower());
                var genres = await _context.Genres.ToListAsync();

                var genresIds = dto.Genres.Select(g => g.genreId).ToList();
                genresIds.Clear();
 

                // Handdel Autthor
                if (author is null)
                {
                    var newAuthor = new Author { AuthorName = dto.AuthorName };
                    await _context.AddAsync(newAuthor);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    if (author.IsDeleted)
                    {
                        author.IsDeleted = false;
                    }
                }

                // Handel Genres
                if (genres is null || genres.Count() < 1)
                {
                    foreach (var genre in dto.Genres)
                    {
                        var newGenre = new Genre { GenreName = genre.GenreName };
                        await _context.Genres.AddAsync(newGenre);
                    }
                }
                else
                {
                    
                    foreach (var genre in dto.Genres)
                    {
                        var existGenre = await _context.Genres.SingleOrDefaultAsync(g => g.GenreName.ToLower() == genre.GenreName.ToLower());
                        if (existGenre is null)
                        {
                            var newGenre = new Genre { GenreName = genre.GenreName };
                            await _context.Genres.AddAsync(newGenre);
                            genresIds.Add(newGenre.Id);
                        }
                        else
                        {
                            if (existGenre.IsDeleted)
                            {
                                existGenre.IsDeleted = false;
                                genresIds.Add(existGenre.Id);
                            }
                        }
                    }
                }

                // Handel Book
                var book = await _context.Books.SingleOrDefaultAsync(b => b.Title.ToLower() == dto.BookTitle.ToLower());
                if (book is null)
                {
                    var newBook = new Book();
                    if (author is null)
                    {
                        var actualAuthor = await _context.Authors.SingleOrDefaultAsync(a => a.AuthorName.ToLower() == dto.AuthorName.ToLower());
                        newBook.AuthorId = actualAuthor.Id;
                    }
                    else
                    {
                        newBook.AuthorId = author.Id;
                    }

                    if(dto.BookCover is not null)
                    {
                        //if (!allowedExtensions.Contains(Path.GetExtension((dto.BookCover.FileName).ToLower())))
                        //    return $"Allowed extensions are {allowedExtensions}";
                        //if (dto.BookCover.Length > allowedSize)
                        //    return "Max file size is 12MB";

                        //using var posterStream = new MemoryStream();
                        //await dto.BookCover.CopyToAsync(posterStream);

                        if(!PictuteHelper.IsAllowedExtension(dto.BookCover))
                            return $"Allowed extensions are {PictuteHelper.allowedExtensions}";
                        if(!PictuteHelper.IsAllowedSize(dto.BookCover))
                            return $"Max file size is {PictuteHelper.allowedSize/(1024*1024)}MB";

                        var file = await PictuteHelper.Upload(dto.BookCover);

                        newBook.Cover = file.ToArray();
                    }

                    newBook.Title = dto.BookTitle;
                    newBook.Description = dto.Description;
                    newBook.PublishYear = dto.PublishYear;

                    await _context.AddAsync(newBook);
                    await AddToGenreListAsync(newBook.Id, genresIds);
                }
                else
                {
                    if (book.IsDeleted)
                    {
                        book.IsDeleted = false;
                        book.Description = dto.Description;
                        book.PublishYear = dto.PublishYear;
                        book.Author.AuthorName = dto.AuthorName;
                        await AddToGenreListAsync(book.Id, genresIds);
                        await _context.SaveChangesAsync();
                        return "Book Added Successfully";
                    }

                    return "Book Already Exists";
                }

                await _context.SaveChangesAsync();
                return "Book Added Successfully";
            }
            catch (Exception e)
            {
                return $"Some Thing Went Wrong And Book Not Added. \n {e.Message}";
            }

        }

        private async Task<string> AddToGenreListAsync(Guid bookId, List<Guid> genresIds)
        {
            try
            {
                for (int i = 0; i < genresIds.Count(); i++)
                {
                    var x = new BookGenres
                    {
                        BookId = bookId,
                        GenreId = genresIds[i]
                    };

                    await _context.BookGenres.AddAsync(x);
                }

                return "Book Added To Genres Successfully";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

    }
}
