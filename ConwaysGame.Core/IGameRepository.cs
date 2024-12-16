namespace ConwaysGame.Core;

public interface IGameRepository
{
    Task<Game> GetGameAsync(int id);
    Task<int> SaveGameAsync(Game game);
}
