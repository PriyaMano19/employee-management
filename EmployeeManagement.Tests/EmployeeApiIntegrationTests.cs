
using System.Net;
using Xunit;

namespace EmployeeManagement.Tests;

public class EmployeeApiIntegrationTests
    : IClassFixture<EmployeeApiTestFixture>
{
    private readonly EmployeeApiTestFixture _fixture;

    public EmployeeApiIntegrationTests(EmployeeApiTestFixture fixture)
    {
        _fixture = fixture;
    }
}