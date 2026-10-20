
using EmployeeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace EmployeeManagement.Tests;

public class EmployeeApiTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres =
        new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("employee_api_test")
            .WithUsername("test_user")
            .WithPassword("TestPassword123!")
            .Build();

    public string ConnectionString => _postgres.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var options = new DbContextOptionsBuilder<EmployeeDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var context = new EmployeeDbContext(options);
        await context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }
}
