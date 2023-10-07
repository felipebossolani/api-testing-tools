using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TasksApi;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddDbContext<ApiContext>(options => options.UseInMemoryDatabase("InMemoryDatabase"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TasksApi", Version = "v1" });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TasksApi v1"));

var users = app.MapGroup("/users");

users.MapGet("/", async (ApiContext context) =>
{
    return await context.Users.ToListAsync();
});
users.MapGet("/{id}", async (ApiContext context, Guid id) =>
{
    return await context.Users.FindAsync(id) is User user ? Results.Ok(user) : Results.NotFound();
});
users.MapPost("/", async (Validated<InsertUserRequest> request, IMapper mapper, ApiContext context) =>
{
    var (isValid, value) = request;

    if (!isValid) return Results.ValidationProblem(request.Errors);

    var user = mapper.Map<User>(value);
    context.Users.Add(user);
    await context.SaveChangesAsync();

    return Results.Created($"/users/{user.Id}", user);
});

users.MapPut("/{id}", async (Guid id, Validated<UpdateUserRequest> request, ApiContext context) =>
{
    var (isValid, value) = request;
    if (!isValid) return Results.ValidationProblem(request.Errors);

    var user = await context.Users.FindAsync(id);
    if (user is null) return Results.NotFound();

    user.Name = value.Name;
    await context.SaveChangesAsync();

    return Results.NoContent();
});

users.MapDelete("/{id}", async (Guid id, ApiContext context) =>
{
    if (!(await context.Users.FindAsync(id) is User user)) 
        return Results.NotFound();
    
    context.Users.Remove(user);
    await context.SaveChangesAsync();
    return Results.NoContent();    
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
