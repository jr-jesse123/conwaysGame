using ConwaysGame.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ConwaysGame.Infra;

public static class IServiceCollectionsExtensions
{
    public static IServiceCollection AddGameRepository(this IServiceCollection services, Action<DbContextOptionsBuilder>? optionsAction)
    {
        services.AddDbContext<GameContext>(optionsAction);
        services.AddScoped<IGameRepository, GameRepository>();
        return services;
    }
}
