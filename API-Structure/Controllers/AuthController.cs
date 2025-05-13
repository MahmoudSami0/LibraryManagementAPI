using API_Structure.Core.DTOs.AuthDtos;
using API_Structure.Core.DTOs.UserDtos;
using API_Structure.Core.Services;
using BookManagementSystem.Core.DTOs.UserDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Structure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserAuthDto>> RegisterAsync(RegisterDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.RegisterAsync(dto);

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("confirm-email")]
        public async Task<ActionResult<RegisterUserAuthDto>> ConfirmEmailAsync([FromQuery] string email, [FromQuery] string token)
        {
            var result = await _authService.ConfirmEmail(email, token);

            return result.Message is not null ? BadRequest(result.Message) : Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserAuthDto>> LoginAsync(LoginDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.LoginAsync(dto);

                if (!result.IsAuthenticated)
                    return BadRequest(result.Message);

                //if(!string.IsNullOrEmpty(result.RefreshToken))
                //    SetRefreshTokenToCookie(result.RefreshToken, result.RefreshTokenExpiration);

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpGet("refresh-token")]
        public async Task<ActionResult<UserAuthDto>> RefreshTokenAsync(string token)
        {
            var result = await _authService.RefreshTokenAsync(token);

            if(!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpPost("revoke-token")]
        public async Task<ActionResult<string>> RevokeTokenAsync(string token)
        {
            var result = await _authService.RevokeTokenAsync(token);

            return !result ? BadRequest("InValid Token!") : Ok("Token Revoked Successfully");
        }


        // Set Refresh Token To Cookie
        /*
        private void SetRefreshTokenToCookie(string refreshToken, DateTime expiresOn) 
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = false,
                Expires = expiresOn.ToLocalTime(),
            };
            Response.Cookies.Append("refreshToken",refreshToken, cookieOptions);
        }
         */

        //[HttpPost("addRoleToUser")]
        //public async Task<ActionResult<string>> AddRoleToUserASync(AddUserRoleDto dto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var result = await _authService.AddRoleAsync(dto);



        //    return result.Contains("Successfully") ? Ok(result) : BadRequest(result);
        //}
    }
}
