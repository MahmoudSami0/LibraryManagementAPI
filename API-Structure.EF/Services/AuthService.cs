using API_Structure.Core;
using API_Structure.Core.Consts;
using API_Structure.Core.DTOs.AuthDtos;
using API_Structure.Core.DTOs.UserDtos;
using API_Structure.Core.Helpers;
using API_Structure.Core.Models;
using API_Structure.Core.Services;
using API_Structure.EF.Data;
using BookManagementSystem.Core.DTOs.UserDtos;
using BookManagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.EF.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly JWT _jwt;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailService _mailService;

        public AuthService(ApplicationDbContext context, IOptions<JWT> jwt, IUnitOfWork unitOfWork, IMailService mailService)
        {
            _context = context;
            _jwt = jwt.Value;
            _unitOfWork = unitOfWork;
            _mailService = mailService;
        }

        public async Task<RegisterUserAuthDto?> RegisterAsync(RegisterDto dto)
        {
            //return await _unitOfWork.Users.AddUserAsync(dto);
            if (!EmailHelper.IsValidEmail(dto.Email))
                return new RegisterUserAuthDto { Message = "Invalid Email Address \n ex: user@example.com" };

            if (await _unitOfWork.Users.FindByEmailAsync(dto.Email.ToLower()) is not null)
                return new RegisterUserAuthDto { Message = "Email Already Exists" };

            if (await _unitOfWork.Users.FindByNameAsync(dto.UserName.ToLower()) is not null)
                return new RegisterUserAuthDto { Message = "Username Already Exists" };

            if (!string.IsNullOrEmpty(dto.Phone))
            {
                if (!PhoneHelper.IsValidPhone(dto.Phone))
                {
                    return new RegisterUserAuthDto { Message = "Invalid Phone Number" };
                }
                if (await _unitOfWork.Users.FindByPhoneAsync(dto.Phone) is not null)
                {
                    return new RegisterUserAuthDto { Message = "Phone Number Already In Use" };
                }
            }

            if (!PasswordHelper.IsValidPassword(dto.Password))
                return new RegisterUserAuthDto
                {
                    Message = "Password must contain at least \n" +
                    "8 Character, One Capital Character, One Small Character, One Special Character. \n" +
                    "EX: Example@123"
                };

            var user = new User
            {
                FirstName = dto.fName,
                LastName = dto.lName,
                Email = dto.Email,
                UserName = dto.UserName,
                Phone = string.IsNullOrEmpty(dto.Phone) ? null : dto.Phone,
                Password = PasswordHelper.HashPassword(dto.Password),
                EmailConfirmationToken = Guid.NewGuid().ToString(),
                EmailConfirmationTokenExpiration = DateTime.UtcNow.AddHours(1),
            };

            var result = await _unitOfWork.Users.AddAsync(user);


            await _mailService.SendEmailAsync(user.Email,user.EmailConfirmationToken);

            //await _unitOfWork.Users.AddToRoleAsync(user, Roles.User);

            return new RegisterUserAuthDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                IsAuthenticated = false,
                Roles = null
            };
        }

        public async Task<RegisterUserAuthDto> ConfirmEmail(string email, string token)
        {
            var user = await _unitOfWork.Users.FindByEmailAsync(email);
            if (user is null || user.EmailConfirmationToken != token || user.EmailConfirmationTokenExpiration < DateTime.UtcNow)
                return new RegisterUserAuthDto { Message = "Invalid confirmation link" };

            user.IsEmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.EmailConfirmationTokenExpiration = null;

            await _unitOfWork.Users.UpdateAsync(user);

            await _unitOfWork.Users.AddToRoleAsync(user, Roles.User);

            return new RegisterUserAuthDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                IsAuthenticated = true,
                Roles = new List<string> { Roles.User }
            };
        }
        
        public async Task<UserAuthDto> LoginAsync(LoginDto dto)
        {
            var authDto = new UserAuthDto();
            if(!EmailHelper.IsValidEmail(dto.Email))
            {
                authDto.Message = "InValid Email";
                return authDto;
            }

            var user = await _unitOfWork.Users.FindAsync(u => u.Email.ToLower() == dto.Email.ToLower(), ["RefreshTokens"]);
            if (user is null || !PasswordHelper.VerifyPassword(dto.Password, user.Password))
            {
                authDto.Message = "Email Or Password Is Incorrect";
                return authDto;
            }
            if(!user.IsEmailConfirmed)
            {
                authDto.Message = "Email Not Confirmed";
                return authDto;
            }
            if(user.IsDeleted)
            {
                authDto.Message = "This Account Is Locked Contact Us To Unlock";
                return authDto;
            }

            var jwtSecurityToken = await GenerateToken(user);
            var rolesList = await _unitOfWork.Roles.GetRolesAsync(user);

            authDto.IsAuthenticated = true;
            authDto.userId = user.Id;
            authDto.Email = user.Email;
            authDto.Username = user.UserName;
            authDto.Phone = user.Phone;
            authDto.Roles = rolesList;
            authDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            if(user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authDto.RefreshToken = activeRefreshToken.Token;
                authDto.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                authDto.RefreshToken = refreshToken.Token;
                authDto.RefreshTokenExpiration = refreshToken.ExpiresOn;

                user.RefreshTokens.Add(refreshToken);

                await _unitOfWork.Users.UpdateAsync(user);
            }
            
            return authDto;
        }
        
        public async Task<UserAuthDto> RefreshTokenAsync(string token)
        {
            var authDto = new UserAuthDto();
            var user = await _unitOfWork.Users.FindAsync(u => u.RefreshTokens.Any(t => t.Token == token), ["RefreshTokens"]);

            if (user is null)
            {
                authDto.IsAuthenticated = false;
                authDto.Message = "Invaild Token";
                return authDto;
            }
            
            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if(!refreshToken.IsActive)
            {
                authDto.IsAuthenticated = false;
                authDto.Message = "Invalid Token";
                return authDto;
            }

            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _unitOfWork.Users.UpdateAsync(user);

            var jwtToken = await GenerateToken(user);
            var roles = await _unitOfWork.Users.GetRolesAsync(user);

            authDto.IsAuthenticated = true;
            authDto.userId = user.Id;
            authDto.Username = user.UserName;
            authDto.Email = user.Email;
            authDto.Roles = roles;
            authDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authDto.RefreshToken = newRefreshToken.Token;
            authDto.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            return authDto;

        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await _unitOfWork.Users.FindAsync(u => u.RefreshTokens.Any(t => t.Token == token), ["RefreshTokens"]);

            if (user is null)
                return false;

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
            if (!refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user);

            return true;
        }

        private async Task<JwtSecurityToken> GenerateToken(User user)
        {

            var roles = await _context.UserRoles.Where(x => x.UserId == user.Id).Select(x => x.Role.RoleName).ToListAsync();

            var roleClaims = new List<Claim>();
            foreach (var role in roles)
                roleClaims.Add(new Claim(ClaimTypes.Role, role));

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.key));
            var Credentials = new SigningCredentials(symmetricSecurityKey,SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            }
            .Union(roleClaims);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.issuer,
                audience: _jwt.audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: Credentials
                );

            return jwtSecurityToken;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);
            return new RefreshToken { Token = Convert.ToBase64String(randomNumber) };
        }

        

        //public async Task<string> AddRoleAsync(AddUserRoleDto dto)
        //{
        //    var user = await _unitOfWork.Users.FindAsync(u => u.Id == dto.UserId, ["UserRoles"]);

        //    if (user is null || !await _unitOfWork.Roles.IsRoleExistsAsync(dto.Role))
        //        return "Invalid User Id Or Role";

        //    if(user.UserRoles.Any(r => r.Role.RoleName.ToLower() == dto.Role.ToLower()))
        //        return "User Already Assigned To This Role" ;

        //    await _unitOfWork.Users.AddToRoleAsync(user, dto.Role);

        //    return "User Assigned To Role Successfully";
        //}
    }
}
