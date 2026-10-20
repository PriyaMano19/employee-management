using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace EmployeeManagement.Tests;

public class AuthApiIntegrationTests : IClassFixture<EmployeeApiTestFixture>
{
    private readonly EmployeeApiTestFixture _fixture;

    public AuthApiIntegrationTests(EmployeeApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Register_WithValidDetails_ReturnsCreated()
    {
        using var factory = new EmployeeApiFactory(_fixture.ConnectionString);
        using var client = factory.CreateClient();

        var request = new
        {
            FullName = "Integration Test User",
            Email = $"test-{Guid.NewGuid():N}@example.com",
            Password = "TestPassword123!"
        };

        var response = await client.PostAsJsonAsync(
            "/api/auth/register", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        using var factory = new EmployeeApiFactory(_fixture.ConnectionString);
        using var client = factory.CreateClient();

        var request = new
        {
            Email = "missing-user@example.com",
            Password = "WrongPassword123!"
        };

        var response = await client.PostAsJsonAsync(
            "/api/auth/login", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

[Fact]
public async Task Login_WithValidCredentials_ReturnsOkWithAccessToken()
{
    using var factory = new EmployeeApiFactory(_fixture.ConnectionString);
    using var client = factory.CreateClient();

    var email = $"login{Guid.NewGuid():N}@example.com";
    const string password = "StrongPassword123!";

    var registerResponse = await client.PostAsJsonAsync(
        "/api/auth/register",
        new
        {
            FullName = "Login Test User",
            Email = email,
            Password = password
        });

    Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

    var loginResponse = await client.PostAsJsonAsync(
        "/api/auth/login",
        new
        {
            Email = email,
            Password = password
        });

    Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

    var responseBody = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();

    Assert.True(responseBody.TryGetProperty("accessToken", out var accessToken));
    Assert.False(string.IsNullOrWhiteSpace(accessToken.GetString()));
}

}
