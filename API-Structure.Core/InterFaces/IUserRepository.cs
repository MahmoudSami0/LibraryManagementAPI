using API_Structure.Core.DTOs.AuthDtos;
using API_Structure.Core.DTOs.UserDtos;
using BookManagementSystem.Core.DTOs.UserDtos;
using BookManagementSystem.Core.Models;


namespace API_Structure.Core.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> FindByEmailAsync(string email);
        Task<User?> FindByNameAsync(string userName);
        Task<User?> FindByPhoneAsync(string phone);
        Task<string?> AddToRoleAsync(User user, string role);
        Task<List<string>> GetRolesAsync(User user);
        Task<string> UpdateUserAsync(Guid id, AddUserDto dto);
    }
}
