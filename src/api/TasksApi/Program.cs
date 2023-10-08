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

app.AddUsersEndpoints();

SeedDatabase(app);
app.Run();

static void SeedDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var apiContext = services.GetRequiredService<ApiContext>();
    DataSeeder.Seed(apiContext);
}
