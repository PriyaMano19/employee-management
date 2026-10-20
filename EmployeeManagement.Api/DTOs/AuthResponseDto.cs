
namespace EmployeeManagement.Api.DTOs;

public record AuthResponseDto(
    string AccessToken,
    DateTime ExpiresAtUtc,
    string Email,
    string FullName
);
