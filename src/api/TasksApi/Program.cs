using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TasksApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApiContext>(options => options.UseInMemoryDatabase("InMemoryDatabase"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TasksApi", Version = "v1" });
});
builder.Services.AddAutoMapper(typeof(Program).Assembly);

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TasksApi v1"));

app.MapGet("/users", async (ApiContext context) =>
{
    return await context.Users.ToListAsync();
});

app.MapGet("/users/{id}", async (ApiContext context, int id) =>
{
    return await context.Users.FindAsync(id) is User user ? Results.Ok(user) : Results.NotFound();
});

app.MapPost("/users", async (ApiContext context, InsertUserCommand command, IMapper mapper) =>
{
    /*
    if (errors.Any())
    {
        return Results.BadRequest(errors);
    }
    */
    var user = mapper.Map<User>(command);

    context.Users.Add(user);
    await context.SaveChangesAsync();

    return Results.Created($"/users/{user.Id}", user);
});

SeedDatabase(app);
app.Run();

static void SeedDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var apiContext = services.GetRequiredService<ApiContext>();
    DataSeeder.Seed(apiContext);
}
