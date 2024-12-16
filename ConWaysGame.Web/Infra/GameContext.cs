using ConwaysGame.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ConwaysGame.Web.Infra;

public class GameContext: DbContext
{
    public GameContext(DbContextOptions<GameContext> options) : base(options)
    {
    }
    public DbSet<Game> Games { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
        });
    }
}
