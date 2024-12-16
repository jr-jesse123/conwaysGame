

/*
 Any live cell with fewer than two live neighbors dies, as if by underpopulation.
Any live cell with two or three live neighbors lives on to the next generation.
Any live cell with more than three live neighbors dies, as if by overpopulation.
Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction.
 */

//TODO: THINK OF SPARSE AND DENSE GRIDS
//TODO: THINK AND TEST PARALLELISM, specially for final state we can share the state for trheads, 




using System.Buffers;

namespace ConwaysGame.Core;


public interface IGameRepository
{
    Task<Game> GetGameAsync(int id);
    Task<int> SaveGameAsync(Game game);
}

public class Game
{
    public int Id { get; set; }

    public int TotalGridCeels { get; private set; }

    private readonly ArrayPool<(int, int)> arrayPool = ArrayPool<(int,int)>.Shared;

    public List<(int x, int y)> LiveCeels { get; private set; } //{ get => _liveCeels; }
    public int Generation { get; private set; } = 0;

    public bool HasStabilized { get; private set; } = false;

    private int MaxGenerations { get; set ; } = 1000;
    private (int x, int y)[] newLiveCellsArray;

    private List<(int, int)> newLiveCells;

    private Dictionary<(int, int), int> positionsWithLiveNeighbors;

    private Game()
    {
    }

    ~Game()
    {
        arrayPool.Return(newLiveCellsArray);
    }
    public Game(IEnumerable<(int x, int y)> board, int gridLenght,  int maxGenerations = 1000)
    {

        if (Math.Sqrt(gridLenght) % 1 != 0 || gridLenght == 1 || gridLenght == 0)
        {
            throw new ArgumentException("The board length must be a perfect square.");
        }

        TotalGridCeels = gridLenght;
        LiveCeels = board.ToList();
        MaxGenerations = maxGenerations;
        LiveCeels.Sort(new CellComparer());

        EnsureInitialized();

    }

    private void EnsureInitialized()
    {
        LiveCeels.Sort(new CellComparer());
        newLiveCellsArray ??= arrayPool.Rent(TotalGridCeels);
        newLiveCells ??= new List<(int, int)>(newLiveCellsArray);
        positionsWithLiveNeighbors ??= new Dictionary<(int, int), int>(TotalGridCeels);
    }


    public void AddNeighbors(int idx, Dictionary<(int,int), int> acc)
    {
        Span<(int x, int y)> neighbors = stackalloc  (int x, int y)[8];
        var (x, y) = LiveCeels[idx];

        neighbors[0] = (x - 1, y);
        neighbors[1] = (x + 1, y);
        neighbors[2] = (x, y - 1);
        neighbors[3] = (x, y + 1);
        neighbors[4] = (x - 1, y - 1);
        neighbors[5] = (x - 1, y + 1);
        neighbors[6] = (x + 1, y - 1);
        neighbors[7] = (x + 1, y + 1);

        for (int i = 0; i < neighbors.Length; i++)
        {
            if (acc.ContainsKey(neighbors[i]))
            {
                acc[neighbors[i]]++;
            }
            else
            {
                acc[neighbors[i]] = 1;
            } 
        }

    }

    public void AdvanceGenerations(int generations)
    {
        
        for (int i = 0; i < generations; i++)
        {
            if (HasStabilized) return;

            AdvanceGeneration();
        }
    }


    public void RunToCompletition()
    {
        while (!HasStabilized)
        {
            AdvanceGeneration();
        }
    }

    private void AdvanceGeneration()
    {
        EnsureInitialized();
        positionsWithLiveNeighbors.Clear();
        newLiveCells.Clear();


        if (Generation >= MaxGenerations)
        {
            throw new MaxGenerationsReachedException("The maximum number of generations has been reached.");
        }

        for (int i = 0; i < LiveCeels.Count; i++)
        {
            AddNeighbors(i, positionsWithLiveNeighbors);
        }
        
        foreach (var (x, y) in positionsWithLiveNeighbors.Keys)
        {
            if(x < 0 || y < 0 || x > Math.Sqrt(TotalGridCeels) - 1 || y > Math.Sqrt(TotalGridCeels) - 1)
                continue;

            var liveNeighbors = positionsWithLiveNeighbors[(x, y)];
            if (liveNeighbors == 3 || (liveNeighbors == 2 && LiveCeels.BinarySearch((x, y)) > -1))
            {
                
                newLiveCells.Add((x, y));
            }
        }

        newLiveCells.Sort(new CellComparer());

        if (newLiveCells.SequenceEqual(LiveCeels))
        {
            HasStabilized = true;
        }

        LiveCeels.Clear();
        LiveCeels.AddRange(newLiveCells);  

        Generation++;
    }

}

public class MaxGenerationsReachedException : Exception
{
    public MaxGenerationsReachedException(string message) : base(message)
    {
    }
}
