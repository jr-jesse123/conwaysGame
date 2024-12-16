using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ConwaysGame.Core;
using ConwaysGame.Web.Infra;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
                services.RemoveAll<GameContext>();

                services.AddGameRepository(options => options.UseSqlite("DataSource=:memory:"));

                var context = services.BuildServiceProvider().GetRequiredService<GameContext>();
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
            [new (1, 1), new (2, 2)],
            9
            );

        // Act
        var response = await client.PostAsync("/game", JsonContent.Create(payload, options: new JsonSerializerOptions() { IncludeFields = true }));


        // Assert
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var gameResponse = Deserialize<StarGameResponse>(content);

        gameResponse.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Get_Game_State_Retuns_NextState()
    {
        // Arrange
        var client = _factory.CreateClient();
        var payload = new StartGameRequest(
            [new (1,0), new (1, 1), new(1, 2)],
            9
            );

        // Act
        var response = await client.PostAsync("/game", JsonContent.Create(payload, options: new JsonSerializerOptions() { IncludeFields = true }));

        var content = await response.Content.ReadAsStringAsync();

        var gameResponse = Deserialize<StarGameResponse>(content);

        var stateResponse = await client.PostAsync($"/game/{gameResponse.Id}", JsonContent.Create(new NextStateRequest(gameResponse.Id, 1)));

        var stateResponseContent = await stateResponse.Content.ReadAsStringAsync();

        var stateObject = Deserialize<NextStateResponse>(stateResponseContent);

        stateObject.CurrentGeneration.Should().Be(1);

        stateObject.LiveCells.Should().BeEquivalentTo([new Coords(0,1), new(1,1), new(2, 1)]);

    }


    [Fact]
    public async Task Get_Game_State_Retuns_WithManyGenerations()
    {
        // Arrange
        var client = _factory.CreateClient();
        var payload = new StartGameRequest(
            [new(1, 0), new(1, 1), new(1, 2)],
            9
            );

        // Act
        var response = await client.PostAsync("/game", JsonContent.Create(payload, options: new JsonSerializerOptions() { IncludeFields = true }));

        var content = await response.Content.ReadAsStringAsync();

        var gameResponse = Deserialize<StarGameResponse>(content);

        var stateResponse = await client.PostAsync($"/game/{gameResponse.Id}", JsonContent.Create(new NextStateRequest(gameResponse.Id, 5)));

        var stateResponseContent = await stateResponse.Content.ReadAsStringAsync();

        var stateObject = Deserialize<NextStateResponse>(stateResponseContent);

        stateObject.CurrentGeneration.Should().Be(5);

        stateObject.LiveCells.Should().BeEquivalentTo([new Coords(0, 1), new(1, 1), new(2, 1)]);

    }


    [Fact]
    public async Task Get_Game_State_ToCompletion_GetsResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        var payload = new StartGameRequest(
            [new(0, 2), new(1, 2), new(2, 2), new(2,1)],
            9
            );

        // Act
        var response = await client.PostAsync("/game", JsonContent.Create(payload, options: new JsonSerializerOptions() { IncludeFields = true }));

        var content = await response.Content.ReadAsStringAsync();

        var gameResponse = Deserialize<StarGameResponse>(content);

        var stateResponse = await client.PostAsync($"/game/{gameResponse.Id}", JsonContent.Create(new NextStateRequest(gameResponse.Id)));

        var stateResponseContent = await stateResponse.Content.ReadAsStringAsync();

        var stateObject = Deserialize<NextStateResponse>(stateResponseContent);

        stateObject.CurrentGeneration.Should().Be(3);

        stateObject.LiveCells.Should().BeEquivalentTo([new Coords(1, 1), new(1, 2), new(2, 1), new(2, 2)]);

    }


    [Fact]
    public async Task Get_Game_State_ToCompletion_GetsError()
    {
        // Arrange
        var client = _factory.CreateClient();
        var payload = new StartGameRequest(
            [new(0, 1), new(1, 1), new(2, 1)],
            9
            );

        // Act
        var response = await client.PostAsync("/game", JsonContent.Create(payload, options: new JsonSerializerOptions() { IncludeFields = true }));

        var content = await response.Content.ReadAsStringAsync();

        var gameResponse = Deserialize<StarGameResponse>(content);

        var stateResponse = await client.PostAsync($"/game/{gameResponse.Id}", JsonContent.Create(new NextStateRequest(gameResponse.Id)));

        var stateResponseContent = await stateResponse.Content.ReadAsStringAsync();

        var stateObject = Deserialize<ErrorDetails>(stateResponseContent);

        stateObject.Details.Should().Be("The maximum number of generations has been reached.");

    }


}