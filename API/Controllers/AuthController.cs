using API.Data;
using API.Entities;
using API.Enums;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly WebContext _context;
        private readonly JwtService _jwtService;
        private readonly PasswordService _passwordService;

        public AuthController(WebContext context, JwtService jwtService, PasswordService passwordService)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordService = passwordService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == registerDto.Email);
            if (exists)
                return BadRequest("Email already in use");

            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                PasswordHash = _passwordService.HashPassword(registerDto.Password),
                Role = UserRole.Student
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _jwtService.Authenticate(loginDto);

            if (result == null)
                return Unauthorized("Invalid email or password");

            return Ok(result);
        }
    }
}