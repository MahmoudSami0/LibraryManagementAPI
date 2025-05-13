using API_Structure.Core;
using API_Structure.Core.DTOs.AuthorDtos;
using API_Structure.Core.DTOs.FavoriteDto;
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
    public class FavoriteController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public FavoriteController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetUserFavorites/{userId}")]
        public async Task<ActionResult<PagedResultDto<FavoriteDto>>> GetUserFavoritesAsync(Guid userId, [FromQuery] PaginationRequestDto dto)
        {
            try
            {
                var query = await _unitOfWork.Favorites.GetAllAsync();
                var totalCount = await query.CountAsync();

                var favorites =
                        await _unitOfWork.Favorites.CustomFindListAsync<Favorite, FavoriteDto>(
                            predicate: f => f.User.Id == userId && !f.IsDeleted,
                            selector: f => new FavoriteDto
                            {
                                favoriteId = f.Id,
                                userId = f.UserId,
                                userName = f.User.UserName,
                                boookId = f.BookId,
                                book = f.Book.Title,
                                AddedOn = f.AddedOn
                            });

                var items = favorites.Skip((dto.PageNumber - 1) * dto.PageSize)
    .                       Take(dto.PageSize);

                var result = new PagedResultDto<FavoriteDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = dto.PageNumber,
                    PageSize = dto.PageSize
                };

                if (result == null)
                    return NotFound("No favorites To Show!");

                return result == null ? NotFound("No favorites To Show!") : Ok(items);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("AddBookToFavorite")]
        public async Task<ActionResult<string>> AddBookToFavoriteAsync(UserFavoriteDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var favorite = await _unitOfWork.Favorites.FindAsync(
                    f => f.UserId == dto.userId && f.BookId == dto.BookId);
                
                if (favorite is not null)
                {
                    if(favorite.IsDeleted)
                    {
                        favorite.IsDeleted = false;
                        await _unitOfWork.Favorites.UpdateAsync(favorite);
                        return Created("https://localhost:7256/api/Favorite/AddBookToFavorite", "Book Added Successfully To Favorite");
                    }
                    return BadRequest("Book Already In Your Favourite");
                }

                var user = await _unitOfWork.Users.GetByIdAsync(dto.userId);
                var book = await _unitOfWork.Books.GetByIdAsync(dto.BookId);
                if (user is null || book is null || user.IsDeleted || book.IsDeleted)
                    return NotFound("User Or Book Not Found!");

                var newFavorite = new Favorite
                {
                    UserId = dto.userId,
                    BookId = dto.BookId
                };

                await _unitOfWork.Favorites.AddAsync(newFavorite);

                return Created("https://localhost:7256/api/Favorite/AddBookToFavorite", "Book Added Successfully To Favorite");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("RemoveBookFromFavorite/{id}")]
        public async Task<ActionResult<string>> RemoveBookFromFavoriteAsync(Guid id)
        {
            var favorite = await _unitOfWork.Favorites.GetByIdAsync(id);

            if (favorite is null || favorite.IsDeleted)
                return NotFound("Favourite Not Found!");

            favorite.IsDeleted = true;
            await _unitOfWork.Favorites.UpdateAsync(favorite);

            return Ok("Favourite Removed Successfully");
        }
    }
}
