
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EmployeeManagement.Tests;

public class EmployeeServiceTests
{
    private static EmployeeDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<EmployeeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new EmployeeDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ValidEmployee_SavesAndReturnsEmployee()
    {
        using var db = CreateDbContext();
        var service = new EmployeeService(db);

        var request = new CreateEmployeeDto
        {
            Name = "Kamal",
            Department = "IT",
            Salary = 70000m
        };

        var result = await service.CreateAsync(request);

        Assert.True(result.Id > 0);
        Assert.Equal("Kamal", result.Name);
        Assert.Equal("IT", result.Department);
        Assert.Equal(70000m, result.Salary);
        Assert.True(result.IsActive);

        var savedEmployee = await db.Employees
            .SingleAsync(e => e.Id == result.Id);

        Assert.Equal("Kamal", savedEmployee.Name);
    }

    [Fact]
    public async Task GetAllAsync_NoEmployees_ReturnsEmptyList()
    {
        using var db = CreateDbContext();
        var service = new EmployeeService(db);

        var result = await service.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_UnknownId_ReturnsNull()
    {
        using var db = CreateDbContext();
        var service = new EmployeeService(db);

        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
    }
}
