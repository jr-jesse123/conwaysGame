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
    public GameRepository()
    {
                
    }
    public Task<Game> GetGameAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveGameAsync(Game game)
    {
        throw new NotImplementedException();
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
