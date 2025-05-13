using API_Structure.Core.DTOs.RoleDtos;
using API_Structure.Core.Models;
using API_Structure.Core.Repositories;
using API_Structure.EF.Data;
using BookManagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.EF.Repositories
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<string>> GetRolesAsync(User user)
        {
            var roles = await CustomFindListAsync<UserRoles, string>(
                predicate: ur => ur.UserId == user.Id,
                selector: r => r.Role.RoleName);

            return roles;
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _context.Roles.SingleOrDefaultAsync(r => r.RoleName.ToLower().StartsWith(roleName.ToLower()) && !r.IsDeleted);
        }

        public async Task<bool> IsRoleExistsAsync(string roleName)
        {
            return await Task.Run(() =>
            _context.Roles.Any(r => r.RoleName.ToLower() == roleName.ToLower()));
        }
    }
}
