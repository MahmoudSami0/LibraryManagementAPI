using API_Structure.Core;
using API_Structure.Core.DTOs.AuthorDtos;
using API_Structure.Core.DTOs.BookGenresDto;
using API_Structure.Core.DTOs.GenereDto;
using API_Structure.Core.DTOs.Pagination;
using BookManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Reflection.Metadata.BlobBuilder;

namespace API_Structure.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GenreController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("Genres")]
        public async Task<ActionResult<PagedResultDto<ReturnedGenreDto>>> GetAllGenresAsync([FromQuery] PaginationRequestDto dto)
        {
            try
            {
                var query = await _unitOfWork.Genres.GetAllAsync();
                var totalCount = await query.CountAsync();



                var genres = await _unitOfWork.Genres.CustomFindListAsync<Genre, ReturnedGenreDto>(
                    predicate: g => !g.IsDeleted,
                    selector: g => new ReturnedGenreDto
                    {
                        genreId = g.Id,
                        genreName = g.GenreName,
                        books = g.BookGenres.Select(x => new CustomBookGenre
                        {
                            bookId = x.BookId,
                            BookTitle = x.Book.Title,
                            Description = x.Book.Description,
                            PublishYear = x.Book.PublishYear,
                            authorId = x.Book.AuthorId,
                            authorName = x.Book.Author.AuthorName
                        }).ToList(),
                    });

                var items = genres.Skip((dto.PageNumber - 1) * dto.PageSize)
                            .Take(dto.PageSize);

                var result = new PagedResultDto<ReturnedGenreDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = dto.PageNumber,
                    PageSize = dto.PageSize
                };

                return result is null ? NotFound("No Genres Found To Show") : Ok(result);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetGenreById/{id}")]
        public async Task<ActionResult<ReturnedGenreDto>> GetGenreByIdAsync(Guid id)
        {
            try
            {
                //Genre? genre = await _unitOfWork.Genres.GetByIdAsync(id);

                var genre = await _unitOfWork.Genres.CustomFindAsync<Genre, ReturnedGenreDto>(
                    predicate: g => g.Id == id && !g.IsDeleted,
                    selector: g => new ReturnedGenreDto
                    {
                        genreId = g.Id,
                        genreName = g.GenreName,
                        books = g.BookGenres.Select(x => new CustomBookGenre
                        {
                            bookId = x.BookId,
                            BookTitle = x.Book.Title,
                            Description = x.Book.Description,
                            PublishYear = x.Book.PublishYear,
                            authorId = x.Book.AuthorId,
                            authorName = x.Book.Author.AuthorName
                        }).ToList()
                    });

                if (genre is null)
                    return NotFound("Genre Not Found!");

                return Ok(genre);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetGenreByName/{genreName}")]
        public async Task<ActionResult<ReturnedGenreDto>> GetGenreByNameAsync(string genreName)
        {
            try
            {
                var genre = await _unitOfWork.Genres.CustomFindAsync<Genre,ReturnedGenreDto>(
                    predicate: g => g.GenreName.ToLower().StartsWith(genreName.ToLower()) && !g.IsDeleted,
                    selector: g => new ReturnedGenreDto
                    {
                        genreId = g.Id,
                        genreName = g.GenreName,
                        books = g.BookGenres.Select(x => new CustomBookGenre
                        {
                            bookId = x.BookId,
                            BookTitle = x.Book.Title,
                            Description = x.Book.Description,
                            PublishYear = x.Book.PublishYear,
                            authorId = x.Book.AuthorId,
                            authorName = x.Book.Author.AuthorName
                        }).ToList()
                    });

                if (genre is null)
                    return NotFound("Genre Not Found!");

                return Ok(genre);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpPost("AddGenre")]
        public async Task<ActionResult<string>> AddGenreAsync(GenreDto dto)
        {
            try
            {
                var genre = await _unitOfWork.Genres.FindAsync(g => g.GenreName.ToLower() == dto.GenreName.ToLower());
                if (genre is not null)
                {
                    if(genre.IsDeleted)
                    {
                        genre.IsDeleted = false;
                        await _unitOfWork.Genres.UpdateAsync(genre);
                    }
                    return BadRequest("Genre Already Exists");
                }

                var newGenre = new Genre()
                {
                    GenreName = dto.GenreName,
                };

                await _unitOfWork.Genres.AddAsync(newGenre);
                return Created("https://localhost:7256/api/Genre/AddGenre", "Genre Added Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpPut("UpdateGenre/{id}")]
        public async Task<ActionResult<string>> UpdateGenreAsync(Guid id, GenreDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var genre = await _unitOfWork.Genres.GetByIdAsync(id);
                if (genre is null || genre.IsDeleted)
                    return NotFound("Genre Not Found!");

                if(genre.GenreName.ToLower() == dto.GenreName.ToLower())
                    return Ok(genre);

                if (await _unitOfWork.Genres.FindAsync(g => g.GenreName.ToLower() == dto.GenreName.ToLower()) is not null)
                {
                    return BadRequest("Genre Already Exists");
                }

                await _unitOfWork.Genres.UpdateAsync(genre);
                return Ok("Genre Updated Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpDelete("DeleteGenre/{id}")]
        public async Task<ActionResult<string>> DeleteGenreAsync(Guid id)
        {
            try
            {
                var genre = await _unitOfWork.Genres.GetByIdAsync(id);
                if (genre is null || genre.IsDeleted)
                    return NotFound("Genre Not Found!");

                genre.IsDeleted = true;
                await _unitOfWork.Genres.UpdateAsync(genre);

                var bookGenres = await _unitOfWork.BookGenres.CustomFindListAsync<BookGenres, BookGenres>(
                    predicate: bg => bg.GenreId == id,
                    selector: bg => new BookGenres
                    {
                        BookId = id,
                        GenreId = bg.GenreId,
                    });

                foreach (var bookGenre in bookGenres)
                {
                    bookGenre.IsDeleted = true;
                    await _unitOfWork.BookGenres.UpdateAsync(bookGenre);
                }

                return Ok("Genre Deleted Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
