using ConwaysGame.Core;
using ConwaysGame.Web.Infra;
using Microsoft.AspNetCore.Diagnostics;
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

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            //options.SerializerOptions.IncludeFields = true;
        });

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

        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.ContentType = "application/json";
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (exceptionHandlerFeature != null)
                {
                    var exception = exceptionHandlerFeature.Error;

                    var statusCode = exception switch
                    {
                        ArgumentNullException => StatusCodes.Status400BadRequest,
                        UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                        MaxGenerationsReachedException => StatusCodes.Status403Forbidden,
                        _ => StatusCodes.Status500InternalServerError
                    };

                    var errorDetails = new ErrorDetails
                    {
                        StatusCode = statusCode,
                        Message = "An error occurred while processing your request.",
                        Details = exception.Message
                    };

                    context.Response.StatusCode = errorDetails.StatusCode;
                    await context.Response.WriteAsync(errorDetails.ToString());
                }
            });
        });

        app.MapPost("/game", async (IGameRepository repository, StartGameRequest request) =>
        {
            var game = new Game(request.LiveCells.Select(c => (c.x, c.y)), request.GameLenght);
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

            game.AdvanceGenerations(request.NumberOfGenerations);

            await repository.SaveGameAsync(game);

            return Results.Ok(new NextStateResponse(game.Id,game.Generation, game.LiveCeels.Select(c => new Coords(c.x, c.y)).ToList()));
            
        })
        .WithName("NextState")
        .WithOpenApi();


        app.Run();
    }
}

public record NextStateRequest(int Id, int NumberOfGenerations = int.MaxValue);

public record Coords(int x, int y);

public record NextStateResponse(int Id, int CurrentGeneration, List<Coords> LiveCells);

public record StartGameRequest(List<Coords> LiveCells, int GameLenght);

     
public record StarGameResponse(int Id);

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string? Details { get; set; } // Opcional, pode incluir informações adicionais

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}
