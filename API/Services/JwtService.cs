using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Data;
using API.Models;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class JwtService
{
    private readonly WebContext _dbcontext;
    private readonly IConfiguration _configuration;
    private readonly PasswordService _passwordService;

    public JwtService(WebContext db, IConfiguration configuration, PasswordService passwordService)
    {
        _dbcontext = db;
        _configuration = configuration;
        _passwordService = passwordService;
    }

    public async Task<AuthResponse?> Authenticate(LoginDto loginDto)
    {
        if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
            return null;

        var user = await _dbcontext.Users
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null)
            return null;

        if (!_passwordService.VerifyPassword(loginDto.Password, user.PasswordHash))
            return null;

        var key = _configuration["JwtConfig:Key"];
        var issuer = _configuration["JwtConfig:Issuer"];
        var audience = _configuration["JwtConfig:Audience"];
        var expiry = DateTime.UtcNow.AddMinutes(
            _configuration.GetValue<int>("JwtConfig:TokenValidityInMinutes")
        );

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiry,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);

        return new AuthResponse
        {
            Token = accessToken,
            Email = user.Email,
            Role = user.Role.ToString(),
            ExpiresIn = (int)(expiry - DateTime.UtcNow).TotalSeconds
        };
    }
}