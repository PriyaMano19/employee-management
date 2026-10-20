
using EmployeeManagement.Api.DTOs;

namespace EmployeeManagement.Api.Services;

public interface IEmployeeService
{
    Task<List<EmployeeResponseDto>> GetAllAsync();
    Task<EmployeeResponseDto?> GetByIdAsync(int id);
    Task<EmployeeResponseDto> CreateAsync(CreateEmployeeDto request);
}
