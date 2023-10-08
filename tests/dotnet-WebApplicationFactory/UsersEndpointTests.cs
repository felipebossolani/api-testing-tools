using System.Net.Http.Json;
using System.Net;
using TasksApi;

namespace dotnet_WebApplicationFactory;

public class UsersEndpointTests : IClassFixture<WebApplicationFactory<TasksApi.Program>>
{
    private readonly WebApplicationFactory<TasksApi.Program> _factory;
    private readonly HttpClient _client;

    public UsersEndpointTests(WebApplicationFactory<TasksApi.Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    private readonly Guid _knownUserId = Guid.Parse("8a44785c-4c85-4e34-9ea3-e904cf2ab4a8");
    private const string _knownUserName = "Felipe";

    [Fact]
    public async Task GetAllUsers_ReturnsSuccessStatusCode()
    {
        var response = await _client.GetAsync("/users");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetUser_ReturnsUser_ForValidId()
    {
        var response = await _client.GetAsync($"/users/{_knownUserId}");
        response.EnsureSuccessStatusCode();

        var user = await response.Content.ReadFromJsonAsync<User>();
        Assert.Equal(_knownUserName, user.Name);
    }

    [Fact]
    public async Task AddUser_ReturnsBadRequest_ForInvalidData()
    {
        var invalidUser = new InsertUserRequest(""); // Empty name to trigger validation.
        var response = await _client.PostAsJsonAsync("/users", invalidUser);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddUser_ReturnsCreated_ForValidData()
    {
        var validUser = new InsertUserRequest("John Doe");
        var response = await _client.PostAsJsonAsync("/users", validUser);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        // Additional checks can be added here.
    }
    
    [Fact]
    public async Task UpdateUser_ReturnsNoContent_ForValidDataAndId()
    {
        var updatedData = new UpdateUserRequest("Updated Felipe");
        var response = await _client.PutAsJsonAsync($"/users/{_knownUserId}", updatedData);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Optionally, get the user and check the updated name.
        var getUserResponse = await _client.GetAsync($"/users/{_knownUserId}");
        getUserResponse.EnsureSuccessStatusCode();
        var user = await getUserResponse.Content.ReadFromJsonAsync<User>();
        Assert.Equal("Updated Felipe", user.Name);
    }
    
    [Fact]
    public async Task UpdateUser_ReturnsNotFound_ForInvalidId()
    {
        var invalidUserId = Guid.NewGuid(); // Assuming this ID doesn't exist.
        var validData = new UpdateUserRequest("Jane Doe");
        var response = await _client.PutAsJsonAsync($"/users/{invalidUserId}", validData);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNoContent_ForValidId()
    {
        // Note: This test will remove the user from the database. Make sure it's desired.
        var response = await _client.DeleteAsync($"/users/{_knownUserId}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNotFound_ForInvalidId()
    {
        var invalidUserId = Guid.NewGuid();
        var response = await _client.DeleteAsync($"/users/{invalidUserId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task TestUserLifecycle_Create_Update_Delete()
    {
        // 1. Create User
        var newUser = new InsertUserRequest("Jane Doe");
        var createResponse = await _client.PostAsJsonAsync("/users", newUser);
        createResponse.EnsureSuccessStatusCode();

        var locationHeader = createResponse.Headers.Location;
        Assert.NotNull(locationHeader);
        var userId = locationHeader.ToString().Split('/').Last();

        // 2. Update User
        var updatedData = new UpdateUserRequest("Jane Doe Updated");
        var updateResponse = await _client.PutAsJsonAsync($"/users/{userId}", updatedData);
        updateResponse.EnsureSuccessStatusCode();

        // Optionally, check if the update was successful
        var getUserResponse = await _client.GetAsync($"/users/{userId}");
        getUserResponse.EnsureSuccessStatusCode();
        var user = await getUserResponse.Content.ReadFromJsonAsync<User>();
        Assert.Equal("Jane Doe Updated", user.Name);

        // 3. Delete User
        var deleteResponse = await _client.DeleteAsync($"/users/{userId}");
        deleteResponse.EnsureSuccessStatusCode();
    }
}
