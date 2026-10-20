
namespace EmployeeManagement.Api.DTOs;

public record EmployeeResponseDto(
    int Id,
    string Name,
    string Department,
    decimal Salary,
    bool IsActive
);
