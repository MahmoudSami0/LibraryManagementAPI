using API_Structure.Core;
using API_Structure.Core.DTOs.AuthorDtos;
using API_Structure.Core.DTOs.Pagination;
using API_Structure.Core.DTOs.ReviewDtos;
using BookManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static System.Reflection.Metadata.BlobBuilder;

namespace API_Structure.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("Reviews")]
        public async Task<ActionResult<PagedResultDto<ReturnedReviewDto>>> GetAllReviewsAsync([FromQuery] PaginationRequestDto dto)
        {
            try
            {
                var query = await _unitOfWork.Reviews.GetAllAsync();
                var totalCount = await query.CountAsync();

                var reviews = await _unitOfWork.Reviews.CustomFindListAsync<Review, ReturnedReviewDto>(
                    predicate: r => !r.IsDeleted,
                    selector: r => new ReturnedReviewDto
                    {
                        reviewId = r.Id,
                        userId = r.UserId,
                        userName = r.User.UserName,
                        bookId = r.BookId,
                        bookTitle = r.Book.Title,
                        reviewContent = r.ReviewContent,
                        addedOn = r.AddedOn,
                        updatedOn = r.UpdatedOn,
                    }
                    );

                var items = reviews.Skip((dto.PageNumber - 1) * dto.PageSize)
                            .Take(dto.PageSize);

                var result = new PagedResultDto<ReturnedReviewDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = dto.PageNumber,
                    PageSize = dto.PageSize
                };

                return result is null ? NotFound("No Reviews To Show") : Ok(result);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetUserReviews/{uid}")]
        public async Task<ActionResult<PagedResultDto<ReturnedReviewDto>>> GetUserReviewsAsync(Guid uid, [FromQuery] PaginationRequestDto dto)
        {
            try 
            {
                var user = await _unitOfWork.Users.GetByIdAsync(uid);
                var query = await _unitOfWork.Reviews.GetAllAsync();
                var totalCount = await query.CountAsync();

                if(user is null || user.IsDeleted)
                    return NotFound("User Not Found!");

                var reviews = await _unitOfWork.Reviews.CustomFindListAsync<Review, ReturnedReviewDto>(
                    predicate: r => r.UserId == user.Id && !r.IsDeleted,
                    selector: r => new ReturnedReviewDto
                    {
                        reviewId = r.Id,
                        userId = r.UserId,
                        userName = r.User.UserName,
                        bookId = r.BookId,
                        bookTitle = r.Book.Title,
                        reviewContent = r.ReviewContent,
                        addedOn = r.AddedOn,
                        updatedOn = r.UpdatedOn
                    });

                var items = reviews.Skip(dto.PageNumber * dto.PageSize)
                    .Take(dto.PageSize);

                var result = new PagedResultDto<ReturnedReviewDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = dto.PageNumber,
                    PageSize = dto.PageSize
                };

                return result is null ? NotFound("No Reviews To Show") : Ok(result);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetBookReviews/{bid}")]
        public async Task<ActionResult<PagedResultDto<ReturnedReviewDto>>> GetBookReviewsAsync(Guid bid, [FromQuery] PaginationRequestDto dto)
        {
            try
            {
                var book = await _unitOfWork.Books.GetByIdAsync(bid);
                var query = await _unitOfWork.Reviews.GetAllAsync();
                var totalCount = await query.CountAsync();

                if (book is null || book.IsDeleted)
                    return NotFound("Book Not Found!");

                var reviews = await _unitOfWork.Reviews.CustomFindListAsync<Review, ReturnedReviewDto>(
                    predicate: r => r.BookId == book.Id && !r.IsDeleted,
                    selector: r => new ReturnedReviewDto
                    {
                        reviewId = r.Id,
                        userId = r.UserId,
                        userName = r.User.UserName,
                        bookId = r.BookId,
                        bookTitle = r.Book.Title,
                        reviewContent = r.ReviewContent,
                        addedOn = r.AddedOn,
                        updatedOn = r.UpdatedOn
                    });

                var items = reviews.Skip(dto.PageNumber * dto.PageSize)
                        .Take(dto.PageSize);

                var result = new PagedResultDto<ReturnedReviewDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = dto.PageNumber,
                    PageSize = dto.PageSize
                };

                return result is null ? NotFound("No Reviews To Show") : Ok(result);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("AddReview")]
        public async Task<ActionResult<string>> AddReviewAsync(ReviewDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _unitOfWork.Users.GetByIdAsync(dto.userId);
                var book = await _unitOfWork.Books.GetByIdAsync(dto.bookId);
                if (user is null || book is null || user.IsDeleted || book.IsDeleted)
                    return NotFound("User Or Book Not Found");

                var review = await _unitOfWork.Reviews.FindAsync(r => r.ReviewContent.ToLower() == dto.reviewContent.ToLower());
                if(review is not null && review.IsDeleted)
                {
                    review.IsDeleted = false;
                    review.AddedOn = DateTime.UtcNow;
                    await _unitOfWork.Reviews.UpdateAsync(review);
                    return Ok("Review Added Successfully");
                }

                var newReview = new Review
                {
                    UserId = dto.userId,
                    BookId = dto.bookId,
                    ReviewContent = dto.reviewContent
                };

                await _unitOfWork.Reviews.AddAsync(newReview);

                return Ok("Review Added Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("UpdateReview/{id}")]
        public async Task<ActionResult<string>> UpdateReviewAsync(Guid id, UpdateReviewDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                Review? review = await _unitOfWork.Reviews.GetByIdAsync(id);
                if (review == null || review.IsDeleted)
                    return NotFound("Review Not Found!");

                review.ReviewContent = dto.reviewContent;
                review.UpdatedOn = DateTime.UtcNow;

                await _unitOfWork.Reviews.UpdateAsync(review);

                return Ok("Review Updated Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("DeleteReview/{id}")]
        public async Task<ActionResult<string>> DeleteReviewAsync(Guid id)
        {
            try
            {
                Review? review = await _unitOfWork.Reviews.GetByIdAsync(id);
                if (review is null || review.IsDeleted)
                    return NotFound("Review Not Found!");

                review.IsDeleted = true;
                await _unitOfWork.Reviews.UpdateAsync(review);

                return Ok("Review Deleted Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
