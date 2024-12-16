using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ConwaysGame.Core;
using ConwaysGame.Web.Infra;

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
                // Remove both the GameRepository and GameContext registrations
                var descriptors = services.Where(
                    d => d.ServiceType == typeof(GameContext) ||
                         d.ServiceType == typeof(IGameRepository) ||
                         d.ServiceType == typeof(DbContextOptions<GameContext>))
                    .ToList();

                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }

                // Add DbContext with in-memory database
                //services.AddDbContext<GameContext>(options =>
                //{
                //    options.UseInMemoryDatabase("TestingDb");
                //});

                services.AddGameRepository(options =>
                {
                    options.UseInMemoryDatabase("TestingDb");
                });

                // Add GameRepository
                //services.AddScoped<IGameRepository, GameRepository>();
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