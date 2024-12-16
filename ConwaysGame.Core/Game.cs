

/*
 RULES: 
    Any live cell with fewer than two live neighbors dies, as if by underpopulation.
    Any live cell with two or three live neighbors lives on to the next generation.
    Any live cell with more than three live neighbors dies, as if by overpopulation.
    Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction.
*/
using System.Buffers;
namespace ConwaysGame.Core;

public class Game
{
    /// <summary>
    /// The unique identifier of the game.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// The number of cells to be created in the grid.
    /// </summary>
    public int TotalGridCeels { get; private set; }
    /// <summary>
    /// Represents the living cells
    /// </summary>
    public List<(int x, int y)> LiveCells { get; private set; }
    /// <summary>
    /// The current generation of the game.
    /// </summary>
    public int Generation { get; private set; } = 0;
    /// <summary>
    /// Indicates if the game has stabilized.
    /// </summary>
    public bool HasStabilized { get; private set; } = false;




    /// <summary>
    /// The array pool to rent and return arrays.
    /// </summary>
    private readonly ArrayPool<(int, int)> arrayPool = ArrayPool<(int,int)>.Shared;
    /// <summary>
    /// The maximum number of generations to run.
    /// </summary>
    private int MaxGenerations { get; set ; } = 1000;
    /// <summary>
    /// The new live cells array.
    /// </summary>
    private (int x, int y)[] newLiveCellsArray;
    /// <summary>
    /// The living cells for next generation
    /// </summary>
    private List<(int, int)> newLiveCells;
    /// <summary>
    /// The positions with live neighbors.
    /// </summary>
    private Dictionary<(int, int), int> positionsWithLiveNeighbors;

    //used for EF
    private Game()
    {
    }

    ~Game()
    {
        // Return the rented array
        arrayPool.Return(newLiveCellsArray);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="coords"></param>
    /// <param name="gridLenght"></param>
    /// <param name="maxGenerations"></param>
    /// <exception cref="BrokenRuleException"></exception>
    public Game(IEnumerable<(int x, int y)> coords, int gridLenght,  int maxGenerations = 1000)
    {

        if (Math.Sqrt(gridLenght) % 1 != 0 || gridLenght == 1 || gridLenght == 0)
        {
            throw new BrokenRuleException("The board length must be a perfect square.");
        }

        TotalGridCeels = gridLenght;
        LiveCells = coords.ToList();
        MaxGenerations = maxGenerations;
        LiveCells.Sort(new CellComparer());

        EnsureInitialized();

    }

    private void EnsureInitialized()
    {
        LiveCells.Sort(new CellComparer()); //Sort the live cells to make the search faster
        newLiveCellsArray ??= arrayPool.Rent(TotalGridCeels);
        newLiveCells ??= new List<(int, int)>(newLiveCellsArray);
        positionsWithLiveNeighbors ??= new Dictionary<(int, int), int>(TotalGridCeels);
    }


    public void AddNeighbors(int idx, Dictionary<(int,int), int> acc)
    {
        Span<(int x, int y)> neighbors = stackalloc  (int x, int y)[8];
        var (x, y) = LiveCells[idx];

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

    /// <summary>
    /// Generates next generations.
    /// </summary>
    /// <param name="generations">number of generations to generate</param>
    public void AdvanceGenerations(int generations)
    {
        for (int i = 0; i < generations; i++)
        {
            if (HasStabilized) return;

            AdvanceGeneration();
        }
    }

    private void AdvanceGeneration()
    {
        EnsureInitialized();
        //clear previous data
        positionsWithLiveNeighbors.Clear();
        newLiveCells.Clear();


        if (Generation >= MaxGenerations) throw new BrokenRuleException("The maximum number of generations has been reached.");

        for (int i = 0; i < LiveCells.Count; i++)
        {
            AddNeighbors(i, positionsWithLiveNeighbors);
        }
        
        foreach (var (x, y) in positionsWithLiveNeighbors.Keys)
        {
            if(x < 0 || y < 0 || x > Math.Sqrt(TotalGridCeels) - 1 || y > Math.Sqrt(TotalGridCeels) - 1)
                continue;

            var liveNeighbors = positionsWithLiveNeighbors[(x, y)];

            if (liveNeighbors == 3 || (liveNeighbors == 2 && LiveCells.BinarySearch((x, y)) > -1))   
                newLiveCells.Add((x, y));
            
        }

        newLiveCells.Sort(new CellComparer()); //Sort the live cells to make the comparison precise.

        if (newLiveCells.SequenceEqual(LiveCells))
        {
            HasStabilized = true;
        }

        LiveCells.Clear();
        LiveCells.AddRange(newLiveCells);  

        Generation++;
    }

}
