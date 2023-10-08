using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace TasksApi;

public static class UserEndpoints
{
    public static void AddUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/users");
        users.MapGet("/", GetAllUsers);
        users.MapGet("/{id}", GetUser);
        users.MapPost("/", AddUser);
        users.MapPut("/{id}", UpdateUser);
        users.MapDelete("/{id}", DeleteUser);
    }

    static async Task<IResult> GetAllUsers(ApiContext context) => 
        TypedResults.Ok(await context.Users.ToListAsync());

    static async Task<IResult> GetUser(ApiContext context, Guid id) => 
        await context.Users.FindAsync(id) is User user ? Results.Ok(user) : Results.NotFound();

    static async Task<IResult> AddUser(Validated<InsertUserRequest> request, IMapper mapper, ApiContext context) {
        var (isValid, value) = request;

        if (!isValid) return Results.ValidationProblem(request.Errors);

        var user = mapper.Map<User>(value);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return Results.Created($"/users/{user.Id}", user);
    }
    static async Task<IResult> UpdateUser(Guid id, Validated<UpdateUserRequest> request, ApiContext context) {
        var (isValid, value) = request;
        if (!isValid) return Results.ValidationProblem(request.Errors);

        var user = await context.Users.FindAsync(id);
        if (user is null) return Results.NotFound();

        user.Name = value.Name;
        await context.SaveChangesAsync();

        return Results.NoContent();
    }
    static async Task<IResult> DeleteUser(Guid id, ApiContext context) {
        if (!(await context.Users.FindAsync(id) is User user))
            return Results.NotFound();

        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return Results.NoContent();
    }
}
