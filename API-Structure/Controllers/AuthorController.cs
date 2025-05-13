using API_Structure.Core;
using API_Structure.Core.DTOs.AuthorDtos;
using API_Structure.Core.DTOs.BookDtos;
using API_Structure.Core.DTOs.GenereDto;
using API_Structure.Core.DTOs.Pagination;
using BookManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Structure.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("Authors")]
        public async Task<ActionResult<PagedResultDto<CustomAuthorDto>>> GetAllAuthorsAsync([FromQuery] PaginationRequestDto dto)
        {
            try
            {
                var query = await _unitOfWork.Authors.GetAllAsync();
                var totalCount = await query.CountAsync();

                var item = await query
                    .Skip((dto.PageNumber - 1) * dto.PageSize)
                    .Take(dto.PageSize)
                    .ToListAsync();


                var authors = await _unitOfWork.Authors.CustomFindListAsync<Author, CustomAuthorDto>(
                    predicate: a => !a.IsDeleted,
                    selector: a => new CustomAuthorDto
                    {
                        authorId = a.Id,
                        name = a.AuthorName,
                        Books = a.Books.Select(b => new BooksForAuthorDto
                        {
                            bookId = b.Id,
                            title = b.Title,
                            Genres = b.BookGenres.Select(bg => new GenreDto {genreId = bg.GenreId ,GenreName = bg.Genre.GenreName }).ToList()
                        }).ToList()
                    }
                    );

                var items = authors.Skip((dto.PageNumber - 1) * dto.PageSize)
                    .Take(dto.PageSize);

                var result = new PagedResultDto<CustomAuthorDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = dto.PageNumber,
                    PageSize = dto.PageSize
                };

                return result is null ? BadRequest("No Authors To Show") : Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetAuthorById/{id}")]
        public async Task<ActionResult<Author>> GetAuthorByIdAsync(Guid id)
        {
            try
            {
                var author = await _unitOfWork.Authors.CustomFindAsync<Author, CustomAuthorDto>(
                    predicate: a => a.Id == id && !a.IsDeleted,
                    selector: a => new CustomAuthorDto
                    {
                        authorId = a.Id,
                        name = a.AuthorName,
                        Books = a.Books.Select(b => new BooksForAuthorDto
                        {
                            bookId = b.Id,
                            title = b.Title,
                            Genres = b.BookGenres.Select(bg => new GenreDto { genreId = bg.GenreId, GenreName = bg.Genre.GenreName }).ToList()
                        }).ToList()
                    }
                    );

                return author is null ? NotFound("Author Not Found!") : Ok(author);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("Search")]
        public async Task<ActionResult<CustomAuthorDto>> GetAuthorByNameAsync(string? name)
        {
            try
            {
                if (name is null)
                {
                    var authors = await _unitOfWork.Authors.CustomFindListAsync<Author, CustomAuthorDto>(
                    predicate: a => !a.IsDeleted,
                    selector: a => new CustomAuthorDto
                    {
                        authorId = a.Id,
                        name = a.AuthorName,
                        Books = a.Books.Select(b => new BooksForAuthorDto
                        {
                            bookId = b.Id,
                            title = b.Title,
                            Genres = b.BookGenres.Select(bg => new GenreDto { genreId = bg.GenreId, GenreName = bg.Genre.GenreName }).ToList()
                        }).ToList()
                    }
                    );

                    return authors is null || authors.Count() < 1 ? BadRequest("No Authors To Show") : Ok(authors);
                }

                var author = await _unitOfWork.Authors.CustomFindAsync<Author, CustomAuthorDto>(
                    predicate: a => a.AuthorName.ToLower().StartsWith(name.ToLower()) && !a.IsDeleted,
                    selector: a => new CustomAuthorDto
                    {
                        authorId = a.Id,
                        name = a.AuthorName,
                        Books = a.Books.Select(b => new BooksForAuthorDto
                        {
                            bookId = b.Id,
                            title = b.Title,
                            Genres = b.BookGenres.Select(bg => new GenreDto { genreId = bg.GenreId, GenreName = bg.Genre.GenreName }).ToList()
                        }).ToList()
                    }
                    );

                return author is null ? BadRequest("Author Not Found") : Ok(author);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpPost("AddAuthor")]
        public async Task<ActionResult<string>> AddAuthorAysnc(AuthorDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var author = await _unitOfWork.Authors.FindAsync(a => a.AuthorName.ToLower() == dto.Name.ToLower());
                if (author is not null)
                {
                    if(author.IsDeleted)
                    {
                        author.IsDeleted = false;
                        await _unitOfWork.Authors.UpdateAsync(author);
                        return Created("https://localhost:7256/api/Author/AddAuthor", "Author Added Successfully");
                    }
                    return BadRequest("Author Already Exists");
                }

                var newAuthor = new Author()
                {
                    AuthorName = dto.Name
                };

                await _unitOfWork.Authors.AddAsync(newAuthor);

                return Created("https://localhost:7256/api/Author/AddAuthor", "Author Added Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpPut("UpdateAuthor/{id}")]
        public async Task<ActionResult<string>> UpdateAuthorAsync(Guid id, AuthorDto dto)
        {
            try
            {
                var author = await _unitOfWork.Authors.GetByIdAsync(id);

                if (author is null || author.IsDeleted) return NotFound("Author Not Found");

                if (!ModelState.IsValid) return BadRequest(ModelState);

                if (await _unitOfWork.Authors.FindAsync(a => a.AuthorName.ToLower() == dto.Name.ToLower()) is not null)
                    return BadRequest("Author Is Already Exists");

                author.AuthorName = dto.Name;

                await _unitOfWork.Authors.UpdateAsync(author);
                return Ok("Author Updated Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpDelete("DeleteAuthor/{id}")]
        public async Task<ActionResult<string>> DeleteAuthorAsync(Guid id)
        {
            try
            {
                var author = await _unitOfWork.Authors.GetByIdAsync(id);

                if (author is null || author.IsDeleted) 
                    return NotFound("Author Not Found!");

                author.IsDeleted = true;
                await _unitOfWork.Authors.UpdateAsync(author);

                var books = author.Books?.Where(b => b.AuthorId == author.Id).ToList();
                if (books is not null || books?.Count > 0)
                {
                    foreach (var book in books)
                    {
                        book.IsDeleted = true;
                        await _unitOfWork.Books.UpdateAsync(book);
                    }
                }
                return Ok("Author Deleted Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
