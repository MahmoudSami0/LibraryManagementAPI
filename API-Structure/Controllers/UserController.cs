using API_Structure.Core;
using API_Structure.Core.DTOs.AuthDtos;
using API_Structure.Core.DTOs.Pagination;
using API_Structure.Core.DTOs.ReviewDtos;
using API_Structure.Core.DTOs.UserDtos;
using BookManagementSystem.Core.DTOs.UserDtos;
using BookManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Structure.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpGet("Users")]
        public async Task<ActionResult<PagedResultDto<ReturnedUserDto>>> GetAllUsersAsync([FromQuery] PaginationRequestDto dto)
        {
            try
            {
                var query = await _unitOfWork.Users.GetAllAsync();
                var totalCount = await query.CountAsync();

                var users = await _unitOfWork.Users.CustomFindListAsync<User, ReturnedUserDto>(
                    predicate: u => !u.IsDeleted,
                    selector: u => new ReturnedUserDto
                    {
                        fName = u.FirstName,
                        lName = u.LastName,
                        email = u.Email,
                        userName = u.UserName,
                        phone = u.Phone,
                        profilePicture = u.Photo
                    });

                var items = users.Skip((dto.PageNumber - 1) * dto.PageSize)
                    .Take(dto.PageSize);

                var result = new PagedResultDto<ReturnedUserDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = dto.PageNumber,
                    PageSize = dto.PageSize
                };

                return result is null ? NotFound("No Users Found To Get!!") : Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpGet("GetUserByEmail/{email}")]
        public async Task<ActionResult<ReturnedUserDto>> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _unitOfWork.Users.FindByEmailAsync(email);

                if (user is null)
                    return NotFound("User Not Found");

                var returnedUser = new ReturnedUserDto()
                {
                    fName = user.FirstName,
                    lName = user.LastName,
                    email = user.Email,
                    userName = user.UserName,
                    phone = user.Phone is null ? null : user.Phone,
                    profilePicture = user.Photo is null ? null : user.Photo
                };

                return Ok(returnedUser);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpGet("GetUserByName/{name}")]
        public async Task<ActionResult<ReturnedUserDto>> GetUserByNameAsync(string name)
        {
            try
            {
                var user = await _unitOfWork.Users.FindByNameAsync(name);

                if (user is null)
                    return NotFound("User Not Found");

                var returnedUser = new ReturnedUserDto()
                {
                    fName = user.FirstName,
                    lName = user.LastName,
                    email = user.Email,
                    userName = user.UserName,
                    phone = user.Phone is null ? null : user.Phone,
                    profilePicture = user.Photo is null ? null : user.Photo
                };

                return Ok(returnedUser);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpGet("GetUserByPhone/{phone}")]
        public async Task<ActionResult<ReturnedUserDto>> GetUserByPhoneAsync(string phone)
        {
            try
            {
                var user = await _unitOfWork.Users.FindByPhoneAsync(phone);

                if (user is null)
                    return NotFound("User Not Found");

                var returnedUser = new ReturnedUserDto()
                {
                    fName = user.FirstName,
                    lName = user.LastName,
                    email = user.Email,
                    userName = user.UserName,
                    phone = user.Phone is null ? null : user.Phone,
                    profilePicture = user.Photo is null ? null : user.Photo
                };

                return Ok(returnedUser);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpGet("GetUserRoles/{id}")]
        public async Task<ActionResult<List<string>>> GetUserRoles(Guid id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user is null || user.IsDeleted)
                    return NotFound("User Not Found!");

                return await _unitOfWork.Users.GetRolesAsync(user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [HttpPut("UpdateUser/{id}")]
        public async Task<ActionResult<string>> UpdateUserAsync(Guid id,[FromForm] AddUserDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _unitOfWork.Users.UpdateUserAsync(id, dto);

                if (result.Contains("Successfully"))
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("DeleteUser/{id}")]
        public async Task<ActionResult<string>> DeleteUserAsync(Guid id)
        {
            try
            {
                User? user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user is null || user.IsDeleted)
                    return NotFound("User Not Found");

                user.IsDeleted = true;
                await _unitOfWork.Users.UpdateAsync(user);

                var userRoles = user.UserRoles.Where(ur => ur.UserId == user.Id).ToList();
                var userBooks = user.UserBooks.Where(ub => ub.UserId == user.Id).ToList();

                foreach(var userRole in userRoles)
                {
                    userRole.IsDeleted = true;
                    await _unitOfWork.UserRoles.UpdateAsync(userRole);
                }

                if (userBooks is not null || userBooks.Count > 0)
                {
                    foreach (var userBook in userBooks)
                    {
                        userBook.IsDeleted = true;
                        await _unitOfWork.UserBooks.UpdateAsync(userBook);
                    }
                }

                return Ok("User Deleted Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

    }
}
