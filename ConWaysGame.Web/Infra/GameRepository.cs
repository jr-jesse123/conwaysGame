using ConwaysGame.Core;

namespace ConwaysGame.Web.Infra;

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
