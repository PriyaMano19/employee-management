
using EmployeeManagement.Api.DTOs;

namespace EmployeeManagement.Api.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request);
}
