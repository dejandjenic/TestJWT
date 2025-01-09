using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace TestJWTLibrary;

public class Generator
{
    private byte[] key;
    private string issuer;
    private string defaultAudience;

    public byte[] Key => key;
    public string Base64Key => Convert.ToBase64String(key);
    
    public Generator(string issuer="https://auth.example.com",string audience = "https://api.example.com")
    {
        var random = new Random();
        key = new byte[32];
        random.NextBytes(key);
        this.issuer = issuer;
        defaultAudience = audience;
    }

    public void ConfigureSigningKeys(IWebHostBuilder builder)
    {
        builder.UseSetting("Authentication:Schemes:Bearer:SigningKeys:0:Id", "2d541ec8");
        builder.UseSetting("Authentication:Schemes:Bearer:SigningKeys:0:Issuer", issuer);
        builder.UseSetting("Authentication:Schemes:Bearer:SigningKeys:0:Value", Base64Key);
        builder.UseSetting("Authentication:Schemes:Bearer:SigningKeys:0:Length", "32");
    }
    
    public void ConfigureAuthentication(IWebHostBuilder builder)
    {
        builder.UseSetting("Authentication:Schemes:Bearer:ValidAudiences:0", defaultAudience);
        builder.UseSetting("Authentication:Schemes:Bearer:ValidIssuer", issuer);
        
        builder.UseSetting("Authentication:Schemes:Bearer:SigningKeys:0:Id", "2d541ec8");
        builder.UseSetting("Authentication:Schemes:Bearer:SigningKeys:0:Issuer", issuer);
        builder.UseSetting("Authentication:Schemes:Bearer:SigningKeys:0:Value", Base64Key);
        builder.UseSetting("Authentication:Schemes:Bearer:SigningKeys:0:Length", "32");
    }
    public string GenerateJwt(string audience = "https://api.example.com",string userId = "john.doe",int expiryInMinutes = 60,params Claim[] additionalClaims)
    {
        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var now = DateTimeOffset.UtcNow;
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Iss, issuer),
            new Claim(JwtRegisteredClaimNames.Aud, audience),
            new Claim(JwtRegisteredClaimNames.Exp, now.AddMinutes(expiryInMinutes).ToUnixTimeSeconds().ToString()),
            new Claim(JwtRegisteredClaimNames.Nbf, now.ToUnixTimeSeconds().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(ClaimTypes.Name, userId)
        };
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims.Union(additionalClaims),
            expires: now.UtcDateTime.AddMinutes(expiryInMinutes),
            signingCredentials: credentials
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
}