using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace ConwaysGame.Tests.Integration;

public class GameApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GameApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_Game_Endpoint_Returns_Success()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsync("/game", JsonContent.Create(new List<(int,int)>()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
} 