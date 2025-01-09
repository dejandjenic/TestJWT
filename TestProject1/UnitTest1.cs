using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;

namespace TestProject1;

public class TestJwtTests
{
    [Fact]
    public void Test()
    {
        var issuer = "https://auth.example.com";
        var audience = "https://api.example.com";
        var generatorObject = new TestJWT.Generator();

        var token = generatorObject.GenerateJwt();
        
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters()
        {
            IssuerSigningKey = new SymmetricSecurityKey(generatorObject.Key),
            ValidIssuer = issuer,
            ValidAudience = audience
        }, out var s);
        principal.Claims.Count().Should().Be(9);
    }
    
    [Fact]
    public void TestNonDefault()
    {
        var issuer = "https://auth2.example.com";
        var audience = "https://api2.example.com";
        var generatorObject = new TestJWT.Generator(issuer, audience);

        var token = generatorObject.GenerateJwt(audience:audience,userId:"some");
        
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters()
        {
            IssuerSigningKey = new SymmetricSecurityKey(generatorObject.Key),
            ValidIssuer = issuer,
            ValidAudience = audience
        }, out var s);
        principal.Claims.Count().Should().Be(9);
        principal.Identity.Name.Should().Be("some");
    }
}