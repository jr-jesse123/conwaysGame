using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ConwaysGame.Core;
using ConwaysGame.Web.Infra;
using Microsoft.Extensions.DependencyInjection;

namespace ConwaysGame.Tests.Integration;

public class GameApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    protected T? Deserialize<T>(string content)
    {
        return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
    }
    public GameApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove both the GameRepository and GameContext registrations
                var descriptors = services.Where(
                    d => d.ServiceType == typeof(GameContext)
                         ||
                         d.ServiceType == typeof(IGameRepository) ||
                         d.ServiceType == typeof(DbContextOptions<GameContext>)
                         )
                    .ToList();

                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }

                services.AddGameRepository(options =>
                {
                    options.UseSqlite("DataSource=:memory:", b => b.MigrationsAssembly("ConwaysGame.WEb"));
                });

                var context = services.BuildServiceProvider().GetRequiredService<GameContext>();
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

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
        var gameResponse = Deserialize<StarGameResponse>(content);

        gameResponse.Id.Should().BeGreaterThan(0);
    }


} 