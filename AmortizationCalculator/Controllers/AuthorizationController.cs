using AmortizationCalc.Interfaces;
using AmortizationCalc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AmortizationCalc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    public class AuthorizationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        public static User user = new User();

        public AuthorizationController(IConfiguration config, IUserService userService, ITokenService tokenService) 
        {
            _config = config;
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = await _userService.Register(request);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(UserDto request)
        {
            var loggedInUser = await _userService.Login(request);

            if (loggedInUser == null)
            {
                return BadRequest("User not found ");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, loggedInUser.PasswordHash))
            {
                return BadRequest("Wrong password ");
            }

            string token = _tokenService.CreateToken(loggedInUser);
            var refreshToken = _tokenService.GenerateRefreshToken();
            _tokenService.SetRefreshToken(loggedInUser, refreshToken);

            user = loggedInUser;

            _tokenService.SetCookie(Response, refreshToken.Token);

            return Ok(token);
        }


        [HttpDelete("delete all")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<User>> Delete()
        {
            await _userService.DeleteAllUsers();
            return Ok(await _userService.GetAllUsers());
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (!user.RefreshToken.Equals(refreshToken))
            {
                return Unauthorized("Invalid Refresh Token");
            }
            else if (user.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Token expires");
            }

            string token = _tokenService.CreateToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            _tokenService.SetRefreshToken(user, newRefreshToken);
            _tokenService.SetCookie(Response, newRefreshToken.Token);

            return Ok(token);
        }
    }
}
