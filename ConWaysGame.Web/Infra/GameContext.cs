using ConwaysGame.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

            entity.Property(e => e.TotalGridCeels);

            entity.Property(e => e.LiveCeels)
                .HasConversion(
                    v => ConvertTupleToString(v),
                    v => ConvertToTupleList(v),
                    new ValueComparer<List<(int x, int y)>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()
                    )
                );


        });
    }

    private static string ConvertTupleToString(List<(int x, int y)> tuples)
    {
        return string.Join(";", tuples.Select(t => $"{t.x},{t.y}"));
    }

    private static List<(int x, int y)> ConvertToTupleList(string value)
    {
        return value.Split(";", StringSplitOptions.RemoveEmptyEntries)
                   .Select(s =>
                   {
                       var parts = s.Split(",");
                       return (int.Parse(parts[0]), int.Parse(parts[1]));
                   })
                   .ToList();
    }
}
