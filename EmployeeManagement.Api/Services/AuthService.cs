
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.Api.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(
        RegisterRequestDto request)
    {
        var email = request.Email.Trim();

        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            throw new InvalidOperationException(
                "An account with this email already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = request.FullName.Trim()
        };

        var result = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                " ",
                result.Errors.Select(error => error.Description));

            throw new InvalidOperationException(errors);
        }

        return GenerateToken(user);
    }

    public async Task<AuthResponseDto?> LoginAsync(
        LoginRequestDto request)
    {
        var email = request.Email.Trim();

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null ||
            !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return null;
        }

        return GenerateToken(user);
    }

    private AuthResponseDto GenerateToken(ApplicationUser user)
    {
        var keyValue = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "JWT signing key is not configured.");

        var issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "JWT issuer is not configured.");

        var audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "JWT audience is not configured.");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(keyValue));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddMinutes(30);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.NameIdentifier, user.Id)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new AuthResponseDto(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt,
            user.Email ?? string.Empty,
            user.FullName);
    }
}
