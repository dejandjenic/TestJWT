using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using FluentAssertions;

namespace TestProject1;

public class APITest
{
    TestJWTLibrary.Generator generator = new();
    
    [Fact]
    public async Task AuthorizedRoleTest()
    {
        await using var factory = new TestServer(generator);
        var client = factory.CreateDefaultClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", generator.GenerateJwt(additionalClaims:new Claim("role","admin")));
        var response = await client.GetAsync("/users");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task ForbbidenRoleTest()
    {
        await using var factory = new TestServer(generator);
        var client = factory.CreateDefaultClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", generator.GenerateJwt());
        var response = await client.GetAsync("/users");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task UnAuthorizedRoleTest()
    {
        await using var factory = new TestServer(generator);
        var client = factory.CreateDefaultClient();
        var response = await client.GetAsync("/users");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task MeTest()
    {
        await using var factory = new TestServer(generator);
        var client = factory.CreateDefaultClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", generator.GenerateJwt());
        var response = await client.GetFromJsonAsync<UserInfo>("/me");
        response.Name.Should().Be("john.doe");
    }
    
    [Fact]
    public async Task MeSpecificNameTest()
    {
        await using var factory = new TestServer(generator);
        var client = factory.CreateDefaultClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", generator.GenerateJwt(userId:"test"));
        var response = await client.GetFromJsonAsync<UserInfo>("/me");
        response.Name.Should().Be("test");
    }
    
    [Fact]
    public async Task MeUnAuthorizedTest()
    {
        await using var factory = new TestServer(generator);
        var client = factory.CreateDefaultClient();
        var response = await client.GetAsync("/me");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task UnAuthorizedTest()
    {
        await using var factory = new TestServer(generator);
        var client = factory.CreateDefaultClient();
        var response = await client.GetAsync("/me");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    } 
}