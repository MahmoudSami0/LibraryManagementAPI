using API_Structure.Core.DTOs.AuthDtos;
using API_Structure.Core.DTOs.UserDtos;
using BookManagementSystem.Core.DTOs.UserDtos;

namespace API_Structure.Core.Services
{
    public interface IAuthService
    {
        Task<RegisterUserAuthDto?> RegisterAsync(RegisterDto dto);
        Task<RegisterUserAuthDto> ConfirmEmail(string email, string token);
        Task<UserAuthDto> LoginAsync(LoginDto dto);
        Task<UserAuthDto> RefreshTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);
    }
}
