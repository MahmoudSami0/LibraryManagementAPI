using API_Structure.Core;
using API_Structure.Core.Consts;
using API_Structure.Core.DTOs.AuthDtos;
using API_Structure.Core.DTOs.UserDtos;
using API_Structure.Core.DTOs.UserRolesDtos;
using API_Structure.Core.Helpers;
using API_Structure.Core.Models;
using API_Structure.Core.Repositories;
using API_Structure.Core.Services;
using API_Structure.EF.Data;
using BookManagementSystem.Core.DTOs.UserDtos;
using BookManagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace API_Structure.EF.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            if(!EmailHelper.IsValidEmail(email))
                return null;

            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower().StartsWith(email.ToLower()) && !u.IsDeleted);
        }

        public async Task<User?> FindByNameAsync(string userName)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.UserName.ToLower().StartsWith(userName.ToLower()) && !u.IsDeleted);
        }

        public async Task<User?> FindByPhoneAsync(string phone)
        {
            if(!PhoneHelper.IsValidPhone(phone))
                return null;

            return await _context.Users.FirstOrDefaultAsync(u => u.Phone!.StartsWith(phone) && !u.IsDeleted);
        }

        public async Task<string?> AddToRoleAsync(User user, string roleName)
        {
            try
            {
                var userr = await FindAsync(u => u.Id == user.Id, ["UserRoles"]);
                var role = await _context.Roles.SingleOrDefaultAsync(r => r.RoleName.ToLower() == roleName.ToLower());

                if (user is null || role is null)
                    return "Invalid User Id Or Role";

                if (user.UserRoles.Any(r => r.RoleId == role.Id))
                    return "User Already Assigned To This Role";

                var userRole = new UserRoles
                {
                    UserId = user.Id,
                    RoleId = role.Id
                };
                await _context.UserRoles.AddAsync(userRole);
                await _context.SaveChangesAsync();
                return "User Added To Role Successfully";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<List<string>> GetRolesAsync(User user)
        {
            return await _context.UserRoles.Where(ur => ur.UserId == user.Id && !ur.IsDeleted).Select(r => r.Role.RoleName).ToListAsync();
        }

        public async Task<string> UpdateUserAsync(Guid id, AddUserDto dto)
        {
            try
            {
                var user = await GetByIdAsync(id);
                if (user is null || user.IsDeleted)
                    return "User Not Found!";

                // Email Validation
                if (!EmailHelper.IsValidEmail(dto.email))
                    return "Invalid Email";
                if (await FindByEmailAsync(dto.email.ToLower()) is not null && user.Email.ToLower() != dto.email.ToLower())
                    return "Eamil Already Assigned To Another User";

                // Username Validation
                if (await FindByNameAsync(dto.userName.ToLower()) is not null && user.UserName.ToLower() != dto.userName.ToLower())
                    return "Username Already Assigned To Another User";

                // Phone Validation
                if (!string.IsNullOrEmpty(dto.phone))
                {
                    if (!PhoneHelper.IsValidPhone(dto.phone))
                        return "Invalid Phone Number";

                    if (await FindByPhoneAsync(dto.phone) is not null && user.Phone != dto.phone)
                        return "Phone Number Already Assgin To Another User";
                }

                if(dto.photo is not null)
                {
                    if (!PictuteHelper.IsAllowedExtension(dto.photo))
                        return $"Allowed extensions are {PictuteHelper.allowedExtensions}";
                    if (!PictuteHelper.IsAllowedSize(dto.photo))
                        return $"Max file size is {PictuteHelper.allowedSize / (1024 * 1024)}MB";

                    var file = await PictuteHelper.Upload(dto.photo);

                    user.Photo = file.ToArray();
                }

                user.FirstName = dto.fName;
                user.LastName = dto.lName;
                user.Email = dto.email;
                user.UserName = dto.userName;
                user.Phone = string.IsNullOrEmpty(dto.phone) ? null : dto.phone;

                await UpdateAsync(user);
                return "User Updated Successfully";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
