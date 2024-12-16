using ConwaysGame.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ConwaysGame.Infra;



public class GameContext: DbContext
{
    public GameContext(DbContextOptions<GameContext> options) : base(options)
    {
    }
    public DbSet<Game> Games { get; set; }
}


public class GameRepository : IGameRepository
{
    private readonly GameContext context;

    public GameRepository(GameContext context)
    {
        this.context = context;
    }
    public async Task<Game> GetGameAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<int> SaveGameAsync(Game game)
    {
        await context.Games.AddAsync(game);
        await context.SaveChangesAsync();
        return game.Id;
    }
}

public static class IServiceCollectionsExtensions
{
    public static IServiceCollection AddGameRepository(this IServiceCollection services, Action<DbContextOptionsBuilder>? optionsAction)
    {
        services.AddDbContext<GameContext>(optionsAction);
        services.AddScoped<IGameRepository, GameRepository>();
        return services;
    }
}
