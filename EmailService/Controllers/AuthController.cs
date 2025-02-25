﻿using EmailService.Dtos;
using EmailService.Entities;
using EmailService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = await authService.RegisterAsync(request);
            if (user == null) { return BadRequest("Email already registered"); }

            return Ok(user);
        }


        [HttpPost("register/admin")]
        public async Task<ActionResult<User>> RegisterAdmin(UserDto request)
        {
            var user = await authService.RegisterAdminAsync(request);
            if (user == null) { return BadRequest("Email already registered"); }

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
        {
            var result = await authService.LoginAsync(request);
            if (result == null) { return BadRequest("Invalid email or password."); }

            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndpoint() {
            return Ok("You are authenticated!");
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndopoint() { return Ok("You are an admin!"); }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request) {
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null) {
                return Unauthorized("Invalid refresh token.");
            }

            return Ok(result);
        }
    }
}
