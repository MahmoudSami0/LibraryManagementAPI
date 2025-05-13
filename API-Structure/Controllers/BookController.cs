using API_Structure.Core;
using API_Structure.Core.DTOs.AuthorDtos;
using API_Structure.Core.DTOs.BookDtos;
using API_Structure.Core.DTOs.CustomDtos;
using API_Structure.Core.DTOs.GenereDto;
using API_Structure.Core.DTOs.Pagination;
using API_Structure.Core.Helpers;
using BookManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Structure.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("Books")]
        public async Task<ActionResult<PagedResultDto<CustomBookDto>>> GetAllBooksAsync(PaginationRequestDto dto)
        {
            try
            {
                var query = await _unitOfWork.Books.GetAllAsync();
                var totalCount = await query.CountAsync();

                var books = await _unitOfWork.Books.CustomFindListAsync<Book, CustomBookDto>(
                    predicate: b => !b.IsDeleted,
                    selector: b => new CustomBookDto
                    {
                        id = b.Id,
                        title = b.Title,
                        description = b.Description,
                        authorId = b.AuthorId,
                        author = b.Author.AuthorName,
                        publishYear = b.PublishYear,
                        genres = b.BookGenres.Select(b => new GenreDto { genreId = b.GenreId, GenreName = b.Genre.GenreName }).ToList(),
                        cover = b.Cover,
                    });

                var items = books.Skip((dto.PageNumber - 1) * dto.PageSize)
                    .Take(dto.PageSize);

                var result = new PagedResultDto<CustomBookDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = dto.PageNumber,
                    PageSize = dto.PageSize
                };

                return result is null ? NotFound("No Books To Show") : Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetBookById/{id}")]
        public async Task<ActionResult<CustomBookDto>> GetBookByIdAsync(Guid id)
        {
            var book = await _unitOfWork.Books.CustomFindAsync<Book, CustomBookDto>(
                predicate: b => b.Id == id && !b.IsDeleted,
                selector: b => new CustomBookDto
                {
                    id = b.Id,
                    title = b.Title,
                    description = b.Description,
                    authorId = b.AuthorId,
                    author = b.Author.AuthorName,
                    publishYear = b.PublishYear,
                    genres = b.BookGenres.Select(b => new GenreDto { genreId = b.GenreId, GenreName = b.Genre.GenreName }).ToList(),
                    cover = b.Cover,
                });

            if (book is null) return BadRequest("Book Not Found!");
            return Ok(book);
        }

        [HttpGet("SearchByName")]
        public async Task<ActionResult<CustomBookDto>> GetBookByNameAsync(string? name)
        {
            try
            {
                if(name is null)
                {
                    var books = await _unitOfWork.Books.CustomFindListAsync<Book, CustomBookDto>(
                    selector: b => new CustomBookDto
                    {
                        id = b.Id,
                        title = b.Title,
                        description = b.Description,
                        authorId = b.AuthorId,
                        author = b.Author.AuthorName,
                        publishYear = b.PublishYear,
                        genres = b.BookGenres.Select(b => new GenreDto {genreId = b.GenreId, GenreName = b.Genre.GenreName }).ToList(),
                        cover = b.Cover,
                    });

                    return books is null || books.Count() < 1 ? NotFound("No Books To Show") : Ok(books);
                }


                var book = await _unitOfWork.Books.CustomFindListAsync<Book, CustomBookDto>(
                        predicate: b => b.Title.ToLower().StartsWith(name.ToLower()) && !b.IsDeleted,
                        selector: b => new CustomBookDto
                        {
                            id = b.Id,
                            title = b.Title,
                            description = b.Description,
                            authorId = b.AuthorId,
                            author = b.Author.AuthorName,
                            publishYear = b.PublishYear,
                            genres = b.BookGenres.Select(bg => new GenreDto {genreId = bg.GenreId, GenreName = bg.Genre.GenreName }).ToList(),
                            cover = b.Cover,
                        });

                if (book is null)
                    return NotFound("Book Not Found!");

                return Ok(book);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("SearchByGenre")]
        public async Task<ActionResult<CustomBookDto>> GetBooksByGenreAsync(string genreName)
        {
            try
            {
                var books = await _unitOfWork.Books.GetBooksByGenreAsync(genreName);

                return books is null || books.Count() < 1 ? NotFound("No Books To Show") : Ok(books);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpPost("AddBook")]
        public async Task<ActionResult<string>> AddBookAsync([FromForm]BookDto dto)
        {
            var result = await _unitOfWork.Books.AddNewBook(dto);
            
            return Ok(result);
        }

        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpPost("AddBookToGenre")]
        public async Task<ActionResult<string>> AddBookToGenre(AddBookToGenreDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                return await _unitOfWork.Books.AddToGenreAsync(dto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpPut("UpdateBook/{id}")]
        public async Task<ActionResult<string>> UpdateBookAsync(Guid id, [FromForm] UpdateBookDto dto)
        {
            try
            {
                Book? book = await _unitOfWork.Books.GetByIdAsync(id);

                if (book is null || book.IsDeleted)
                    return NotFound("Book Not Found!");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // TODO: Handel If Author Not Found Add It To Authors Table
                var author = await _unitOfWork.Authors.FindAsync(a => a.AuthorName.ToLower() == dto.AuthorName.ToLower());

                if(author is null)
                {
                    var newAuthor = new Author { AuthorName = dto.AuthorName };
                    await _unitOfWork.Authors.AddAsync(newAuthor);
                    book.AuthorId = newAuthor.Id;
                }
                else
                    book.AuthorId = author.Id;

                if (dto.BookCover is not null)
                {

                    if (!PictuteHelper.IsAllowedExtension(dto.BookCover))
                        return BadRequest($"Allowed extensions are {PictuteHelper.allowedExtensions}");
                    if (!PictuteHelper.IsAllowedSize(dto.BookCover))
                        return BadRequest($"Max file size is {PictuteHelper.allowedSize / (1024 * 1024)}MB");

                    var file = await PictuteHelper.Upload(dto.BookCover);

                    book.Cover = file.ToArray();
                }

                book.Title = dto.BookTitle;
                book.Description = dto.Description;
                book.PublishYear = dto.PublishYear;   

                await _unitOfWork.Books.UpdateAsync(book);

                return Ok("Book updated Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpDelete("DeleteBook/{id}")]
        public async Task<ActionResult<string>> DeleteBookAsync(Guid id)
        {
            try
            {
                Book? book = await _unitOfWork.Books.GetByIdAsync(id);
                if (book is null || book.IsDeleted)
                    return NotFound("Book Not Found!");

                book.IsDeleted = true;
                await _unitOfWork.Books.UpdateAsync(book);

                var bookGenres = book.BookGenres.Where(bg => bg.BookId == id).ToList();
                var userBooks = book.UserBooks?.Where(ub => ub.BookId == id).ToList();

                foreach (var bookGenre in bookGenres)
                {
                    bookGenre.IsDeleted = true;
                    await _unitOfWork.BookGenres.UpdateAsync(bookGenre);
                }

                if (userBooks is not null || userBooks?.Count > 0)
                {
                    foreach (var userBook in userBooks)
                    {
                        userBook.IsDeleted = true;
                        await _unitOfWork.UserBooks.UpdateAsync(userBook);
                    }
                }

                return Ok("Book Deleted Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
