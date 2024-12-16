using ConwaysGame.Core;
using Microsoft.EntityFrameworkCore;

namespace ConwaysGame.Web.Infra;

public class GameContext: DbContext
{
    public GameContext(DbContextOptions<GameContext> options) : base(options)
    {
    }
    public DbSet<Game> Games { get; set; }
}
