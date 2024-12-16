using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ConwaysGame.Infra;

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
                // Remove the real DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<GameContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database for testing
                services.AddDbContext<GameContext>(options =>
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
        var payload = new StartGameRequest(new List<(int, int)> { (1, 1), (2, 2) });

        // Act
        var response = await client.PostAsync("/game", JsonContent.Create(payload));

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var gameResponse = JsonSerializer.Deserialize<StarGameResponse>(content);
        gameResponse!.Id.Should().BeGreaterThan(0);

        // Verify the data was saved
        //using var scope = _factory.Services.CreateScope();
        //var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
        //var savedGame = await dbContext.Games
        //    .Include(g => g.LiveCells)
        //    .FirstOrDefaultAsync(g => g.Id == gameResponse.Id);
        
        //savedGame.Should().NotBeNull();
        //savedGame!.LiveCells.Should().HaveCount(2);
    }
} 