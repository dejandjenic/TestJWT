# TestJWT

TestJWT is a library created to support working with dotnet api in test environment.

## Usage

* Install TestJWT from nuget

inside your tests instantiate generator class

```csharp
TestJWT.Generator generator = new();
```

setup test server 

```csharp
public class TestServer(TestJWT.Generator generator) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        generator.ConfigureAuthentication(builder);
    }
}
```

create test

```csharp
[Fact]
public async Task AuthorizedTest()
{
    await using var factory = new TestServer(generator);
    var client = factory.CreateDefaultClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", generator.GenerateJwt());
    var response = await client.GetAsync("/users");
    response.StatusCode.Should().Be(HttpStatusCode.OK);
}
```

## Default values

by default generator assumes these value
* issuer="https://auth.example.com"
* audience = "https://api.example.com"

GenerateJwt method by default assumes these settings

* audience = "https://api.example.com"
* userId = "john.doe"
* 60

list of additional claims can be passed to GenerateJWT method