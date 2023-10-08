namespace dotnet_WebApplicationFactory;

/*

public class UsersEndpointTests : IClassFixture<WebApplicationFactory<TasksApi.Startup>>
{
    private readonly WebApplicationFactory<TasksApi.Startup> _factory;
    private readonly HttpClient _client;

    public UsersEndpointTests(WebApplicationFactory<TasksApi.Startup> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAllUsers_ReturnsSuccessStatusCode()
    {
        // Arrange
        var url = "/users";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    // Você pode adicionar mais testes para os outros endpoints aqui...
}
*/