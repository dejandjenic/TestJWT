using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TestProject1;

public class TestServer(TestJWTLibrary.Generator generator) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        generator.ConfigureAuthentication(builder);
        builder.UseEnvironment("Test");
    }
}