using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ConwaysGame.Infra;
using ConwaysGame.Core;

namespace ConwaysGame.Tests.Integration;

public class GameApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GameApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the GameRepository registration that uses SQLite
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(GameContext));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add GameRepository with in-memory database
                services.AddGameRepository(options =>
                {
                    options.UseInMemoryDatabase("TestingDb");
                });
            });
        });
    }

    [Fact]
    public async Task Post_Game_Endpoint_Returns_Success()
    {
        // Arrange
        var client = _factory.CreateClient();
        var payload = new StartGameRequest(
            [(1, 1), (2, 2)],
            9
            );

        // Act
        var response = await client.PostAsync("/game", JsonContent.Create(payload));

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var gameResponse = JsonSerializer.Deserialize<StarGameResponse>(content);
        gameResponse!.Id.Should().BeGreaterThan(0);
    }
} 