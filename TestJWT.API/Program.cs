var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddAuthentication("Bearer").AddJwtBearer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/users", () => new UserInfo[]
    {
        new()
        {
            Name = "me"
        },
        new()
        {
            Name = "you"
        }
    })
    .WithName("users")
    .WithOpenApi()
    .RequireAuthorization(p =>
    {
        p.RequireRole("admin");
    });

app.MapGet("/me", (HttpContext context) => new UserInfo
    {
        Name = context.User.Identity.Name
    })
    .WithName("me")
    .WithOpenApi()
    .RequireAuthorization();

app.Run();

public class UserInfo
{
    public string Name { get; set; }
}

public partial class Program{}