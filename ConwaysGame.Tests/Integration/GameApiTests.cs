using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ConwaysGame.Tests.Integration;

public class GameApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GameApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_Game_Endpoint_Returns_Success()
    {
        // Arrange
        var client = _factory.CreateClient();

        var paylodad = new StartGameRequest([]);

        // Act
        var response = await client.PostAsync("/game", JsonContent.Create(paylodad));

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        JsonSerializer.Deserialize<StarGameResponse>(content)!.Id
            .Should().NotBe(0);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
} 