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



        app.MapPost("/game", async (IGameRepository repository, StartGameRequest request) =>
        {
            var game = new Game(request.LiveCells, request.GameLenght);
            var id = await repository.SaveGameAsync(game);
            var uri = $"/game/{id}";
            return Results.Created(uri, new StarGameResponse(id));
        })
        .WithName("StartGame")
        .WithOpenApi();
        

        app.MapPost("/game/{Id:int}", async (IGameRepository repository, NextStateRequest request) =>
        {
            var game = await repository.GetGameAsync(request.Id);

            if (game is null)
            {
                return Results.NotFound();
            }

            game.AdvanceGeneration();

            return Results.Ok(new NextStateResponse(game.Id,game.Generation, game.LiveCeels));
            
        })
        .WithName("NextState")
        .WithOpenApi();


        app.Run();
    }
}

public record NextStateRequest(int Id);

public record NextStateResponse(int Id, int CurrentGeneration, List<(int, int)> LiveCells);

public record StartGameRequest(List<(int, int)> LiveCells, int GameLenght);

     
public record StarGameResponse(int Id);

