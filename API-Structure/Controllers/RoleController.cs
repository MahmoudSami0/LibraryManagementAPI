using API_Structure.Core;
using API_Structure.Core.DTOs.AuthDtos;
using API_Structure.Core.DTOs.AuthorDtos;
using API_Structure.Core.DTOs.Pagination;
using API_Structure.Core.DTOs.RoleDtos;
using API_Structure.Core.DTOs.UserRolesDtos;
using API_Structure.Core.Models;
using BookManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Reflection.Metadata.BlobBuilder;

namespace API_Structure.Controllers
{
    [Authorize(Policy = "RequireManagerOrAdminRole")]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        [HttpGet("Roles")]
        public async Task<ActionResult<PagedResultDto<ReturnedRoleDto>>> GetAllRolesAsync([FromQuery] PaginationRequestDto dto)
        {
            try
            {
                var query = await _unitOfWork.Books.GetAllAsync();
                var totalCount = await query.CountAsync();

                var roles = await _unitOfWork.Roles.CustomFindListAsync<Role,ReturnedRoleDto>(
                    predicate: r => !r.IsDeleted,
                    selector: r => new ReturnedRoleDto
                    {
                        roleId = r.Id,
                        roleName = r.RoleName,
                        UsersInRole = r.UserRoles!.Select(ur => new UserRolesDto
                        {
                            userId = ur.UserId,
                            userName = ur.User.UserName,
                            email = ur.User.Email,
                        }).ToList()
                    });

                var items = roles.Skip((dto.PageNumber - 1) * dto.PageSize)
                            .Take(dto.PageSize);

                var result = new PagedResultDto<ReturnedRoleDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = dto.PageNumber,
                    PageSize = dto.PageSize
                };

                return result is null ? BadRequest("No Roles Found To Show!") : Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetRoleById/{id}")]
        public async Task<ActionResult<RoleDto>> GetRoleById(Guid id)
        {
            try
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(id);
                if (role is null || role.IsDeleted)
                    return NotFound("Role Not Found!");

                return Ok(role);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetRoleByName/{name}")]
        public async Task<ActionResult<RoleDto>> GetRoleByName(string name)
        {
            try
            {
                var role = await _unitOfWork.Roles.GetRoleByNameAsync(name.ToLower());
                if (role is null)
                    return NotFound("Role Not Found!");

                return Ok(role);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("AddRole")]
        public async Task<ActionResult<string>> AddRoleAsync(RoleDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                
                var role = await _unitOfWork.Roles.FindAsync(r => r.RoleName.ToLower() == dto.roleName.ToLower());
                if (role is not null)
                {
                    if (role.IsDeleted)
                    {
                        role.IsDeleted = false;
                        await _unitOfWork.Roles.UpdateAsync(role);
                        return Created("https://localhost:7256/api/Role/AddRole", "Role Added Successfully");
                    }
                    return BadRequest("Role Already Exists!");
                }

                Role newRole = new Role()
                {
                    RoleName = dto.roleName,
                };

                await _unitOfWork.Roles.AddAsync(newRole);
                return Created("https://localhost:7256/api/Role/AddRole", "Role Added Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("AddRoleToUser")]
        public async Task<ActionResult<string>> AddRoleToUserAsync(Guid userId, string roleName)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            var result = await _unitOfWork.Users.AddToRoleAsync(user, roleName);

            return result.Contains("Successfully") ? Ok(result) : BadRequest(result);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("UpdateRole/{id}")]
        public async Task<ActionResult<string>> UpdateRoleAsync(Guid id, RoleDto dto)
        {
            try
            {
                Role? role = await _unitOfWork.Roles.GetByIdAsync(id);
                if (role is null || role.IsDeleted)
                    return NotFound("Role Not Found");

                if(role.RoleName.ToLower() == dto.roleName.ToLower())
                    return NoContent();

                var roleExist = await _unitOfWork.Roles.FindAsync(r => r.RoleName.ToLower() == dto.roleName.ToLower());
                if (roleExist is not null)
                {
                    if (roleExist.IsDeleted)
                    {
                        roleExist.IsDeleted = false;
                        await _unitOfWork.Roles.UpdateAsync(roleExist);
                        return Ok("Role Updated Successfully");
                    }
                    return BadRequest("Role Already Exists!");
                }

                role.RoleName = dto.roleName;
                await _unitOfWork.Roles.UpdateAsync(role);
                return Ok("Role Updated Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("DeleteRole/{id}")]
        public async Task<ActionResult<string>> DeleteRoleAsync(Guid id)
        {
            try
            {
                Role? role = await _unitOfWork.Roles.GetByIdAsync(id);
                if (role is null || role.IsDeleted)
                    return NotFound("Role Not Found!");

                role.IsDeleted = true;
                await _unitOfWork.Roles.UpdateAsync(role);
                
                var userRoles = role.UserRoles?.Where(ur => ur.UserId == role.Id).ToList();
                if (userRoles is not null || userRoles?.Count > 0)
                {
                    foreach (var userRole in userRoles)
                    {
                        userRole.IsDeleted = true;
                        await _unitOfWork.UserRoles.UpdateAsync(userRole);
                    }
                }

                return Ok("Role Deleted Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }


        }
    }
}
