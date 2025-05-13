using API_Structure.Core;
using API_Structure.Core.DTOs.Pagination;
using API_Structure.Core.DTOs.ReviewDtos;
using API_Structure.Core.DTOs.UserBooksDtos;
using BookManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Structure.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserBooksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserBooksController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetUserBooks/{uid}")]
        public async Task<ActionResult<PagedResultDto<UserBooksDto>>> GetUserBooksAsync(Guid uid, [FromQuery] PaginationRequestDto dto)
        {
            try
            {
                User? user = await _unitOfWork.Users.GetByIdAsync(uid);
                var query = await _unitOfWork.UserBooks.GetAllAsync();
                var totalCount = await query.CountAsync();

                if (user is null || user.IsDeleted)
                    return NotFound("User Not Found");

                var userBooks = await _unitOfWork.UserBooks.CustomFindListAsync<UserBooks, UserBooksDto>(
                    predicate: ub => ub.UserId == user.Id && !ub.Book.IsDeleted,
                    selector: ub => new UserBooksDto
                    {
                        bookId = ub.BookId,
                        bookTitle = ub.Book.Title,
                        bookDescription = ub.Book.Description,
                        ReservedOn = ub.ReservedOn,
                        ReservedTo = ub.ReservedTo,
                        cover = ub.Book.Cover
                    });

                var items = userBooks.Skip(dto.PageNumber * dto.PageSize)
                    .Take(dto.PageSize);

                var result = new PagedResultDto<UserBooksDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = dto.PageNumber,
                    PageSize = dto.PageSize
                };

                return result is null ? NotFound("No Books Found") : Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("AddBookToUser")]
        public async Task<ActionResult<string>> AddBookToUserAsync(AddUserBookDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _unitOfWork.Users.GetByIdAsync(dto.userId);
                var book = await _unitOfWork.Books.GetByIdAsync(dto.bookId);
                if (user is null || book is null ||user.IsDeleted || book.IsDeleted)
                    return NotFound("User Or Book Not Found");

                var userBook = await _unitOfWork.UserBooks.FindAsync(ub => ub.UserId == user.Id && ub.BookId == book.Id);

                if(userBook is not null)
                {
                    if (userBook.IsDeleted)
                    {
                        userBook.IsDeleted = false;
                        userBook.ReservedOn = DateOnly.FromDateTime(DateTime.UtcNow);
                        userBook.ReservedTo = dto.reservedTo;
                        
                        await _unitOfWork.UserBooks.UpdateAsync(userBook);
                        return Ok("Book Added To User Successfully");
                    }
                    return BadRequest("User Already Have This Book");  
                }

                var newUserBook = new UserBooks
                {
                    UserId = dto.userId,
                    BookId = dto.bookId,
                    ReservedTo = dto.reservedTo,
                };

                await _unitOfWork.UserBooks.AddAsync(newUserBook);
                return Ok("Book Added To User Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("RemoveBookFromUser/{uid}/{bid}")]
        public async Task<ActionResult<string>> RemoveBookFromUserAsync(Guid uid, Guid bid)
        {
            try
            {
                if (await _unitOfWork.Users.GetByIdAsync(uid) is null ||
                    await _unitOfWork.Books.GetByIdAsync(bid) is null)
                    return BadRequest("User Or Book Not Found");

                var userBook = await _unitOfWork.UserBooks.FindAsync(ub => ub.UserId == uid && ub.BookId == bid);

                if (userBook is null)
                    return BadRequest("Book Not Reserved To This User");

                userBook.IsDeleted = true;
                await _unitOfWork.UserBooks.UpdateAsync(userBook);

                return Ok("Book Removed From User Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
