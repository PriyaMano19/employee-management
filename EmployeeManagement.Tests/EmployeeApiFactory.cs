
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace EmployeeManagement.Tests;

public class EmployeeApiFactory(string connectionString)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = connectionString,
                    ["Jwt:Key"] = "TestOnlySigningKey_123456789012345678901234567890",
                    ["Jwt:Issuer"] = "EmployeeManagement.Api",
                    ["Jwt:Audience"] = "EmployeeManagement.React"
                });
        });
    }
}
