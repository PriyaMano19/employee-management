
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace EmployeeManagement.Tests;

public class PostgreSqlIntegrationTests
{
    [Fact]
    public async Task CreateEmployee_WithRealPostgreSql_SavesEmployee()
    {
        await using var postgres = new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("employee_test")
            .WithUsername("test_user")
            .WithPassword("TestPassword123!")
            .Build();

        await postgres.StartAsync();

        var options = new DbContextOptionsBuilder<EmployeeDbContext>()
            .UseNpgsql(postgres.GetConnectionString())
            .Options;

        await using var context = new EmployeeDbContext(options);

        await context.Database.EnsureCreatedAsync();

        var service = new EmployeeService(context);

        var request = new CreateEmployeeDto
        {
            Name = "Test Employee",
            Department = "Engineering",
            Salary = 75000m
        };

        var result = await service.CreateAsync(request);

        Assert.True(result.Id > 0);
        Assert.Equal("Test Employee", result.Name);
        Assert.Equal("Engineering", result.Department);
        Assert.Equal(75000m, result.Salary);

        var savedEmployee = await context.Employees
            .AsNoTracking()
            .SingleOrDefaultAsync(employee => employee.Id == result.Id);

        Assert.NotNull(savedEmployee);
        Assert.Equal("Test Employee", savedEmployee.Name);
    }
}
