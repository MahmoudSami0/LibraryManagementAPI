using API_Structure.Core.Models;
using BookManagementSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.Core.Repositories
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        Task<List<string>> GetRolesAsync(User user);
        Task<Role?> GetRoleByNameAsync(string roleName);
        Task<bool> IsRoleExistsAsync(string roleName);
    }
}
