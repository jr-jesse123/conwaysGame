using ConwaysGame.Core;
using ConwaysGame.Web.Infra;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add EF Core
        builder.Services.AddGameRepository(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            options.UseSqlite(connectionString);
        });

        var app = builder.Build();

        // Ensure database is created and migrated
      

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        

        app.MapPost("/game", async (IGameRepository repository, StartGameRequest startGameDto) =>
        {
            var game = new Game(startGameDto.LiveCells, startGameDto.GameLenght);
            var id = await repository.SaveGameAsync(game);
            return Results.Ok(new StarGameResponse { Id = id});
        })
        .WithName("StartGame")
        .WithOpenApi();


        app.Run();
    }
}

public record StartGameRequest(List<(int, int)> LiveCells, int GameLenght);

     
public class StarGameResponse
{
    public required int Id { get; set; }
}

