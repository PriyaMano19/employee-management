
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Services;

public class EmployeeService : IEmployeeService
{
    private readonly EmployeeDbContext _db;

    public EmployeeService(EmployeeDbContext db)
    {
        _db = db;
    }

    public async Task<List<EmployeeResponseDto>> GetAllAsync()
    {
        return await _db.Employees
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Select(e => new EmployeeResponseDto(
                e.Id,
                e.Name,
                e.Department,
                e.Salary,
                e.IsActive))
            .ToListAsync();
    }

    public async Task<EmployeeResponseDto?> GetByIdAsync(int id)
    {
        return await _db.Employees
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EmployeeResponseDto(
                e.Id,
                e.Name,
                e.Department,
                e.Salary,
                e.IsActive))
            .FirstOrDefaultAsync();
    }

    public async Task<EmployeeResponseDto> CreateAsync(
        CreateEmployeeDto request)
    {
        var employee = new Employee
        {
            Name = request.Name.Trim(),
            Department = request.Department.Trim(),
            Salary = request.Salary,
            IsActive = true
        };

        _db.Employees.Add(employee);
        await _db.SaveChangesAsync();

        return new EmployeeResponseDto(
            employee.Id,
            employee.Name,
            employee.Department,
            employee.Salary,
            employee.IsActive);
    }
}
