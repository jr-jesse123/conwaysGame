

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


public class Game
    {
    private readonly int _gridSideLenght;
    //private Span<(int x, int y)> _board;
    private List<(int x, int y)> _board;

    ArrayPool<(int, int)> arrayPool = ArrayPool<(int,int)>.Shared;
        //public readonly ReadOnlySpan<(int x, int y)> Board { get => _board;  }
        public  List<(int x, int y)> LiveCeels { get => _board; }
    public int Generation { get; private set; } = 0;

        public bool HasStabilized { get; private set; } = false;

        private int MaxGenerations { get; init ; } = 1000;
        private (int x, int y)[] newLiveCellsArray;

        private List<(int, int)> newLiveCells;

        private Dictionary<(int, int), int> positionsWithLiveNeighbors;


    public Game()
    {
        throw new NotSupportedException("You must provide a board to start the game.");
    }

    ~Game()
    {
        arrayPool.Return(newLiveCellsArray);
    }
    public Game(IEnumerable<(int x, int y)> board, int gridLenght,  int maxGenerations = 1000)
    {

        if (Math.Sqrt(gridLenght) % 1 != 0)
        {
            throw new ArgumentException("The board length must be a perfect square.");
        }
        //board.Sort(new CellComparer());

        MaxGenerations = maxGenerations;
        _gridSideLenght = gridLenght;
        _board = board.ToList();
        _board.Sort(new CellComparer());
        
        newLiveCellsArray = arrayPool.Rent(_gridSideLenght);
        newLiveCells = new List<(int, int)>(newLiveCellsArray);
        positionsWithLiveNeighbors = new Dictionary<(int, int), int>(_gridSideLenght);

    }

    public void AddNeighbors(int idx, Dictionary<(int,int), int> acc)
    {
        Span<(int x, int y)> neighbors = stackalloc  (int x, int y)[8];
        var (x, y) = _board[idx];

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
            AdvanceGeneration();
        }
    }

    public void AdvanceGeneration()
    {
        if (Generation >= MaxGenerations)
        {
            throw new NotSupportedException("The maximum number of generations has been reached.");
        }

        positionsWithLiveNeighbors.Clear();

        for (int i = 0; i < LiveCeels.Count; i++)
        {
            AddNeighbors(i, positionsWithLiveNeighbors);
        }

        

        
        newLiveCells.Clear();
        foreach (var (x, y) in positionsWithLiveNeighbors.Keys)
        {
            if(x < 0 || y < 0 || x > Math.Sqrt(_gridSideLenght) - 1 || y > Math.Sqrt(_gridSideLenght) - 1)
                continue;

            var liveNeighbors = positionsWithLiveNeighbors[(x, y)];
            if (liveNeighbors == 3 || (liveNeighbors == 2 && LiveCeels.BinarySearch((x, y)) > -1))
            {
                
                newLiveCells.Add((x, y));
            }
        }

        if (newLiveCells.SequenceEqual(LiveCeels))
        {
            HasStabilized = true;
        }

        _board.Clear();
        _board.AddRange(newLiveCells);  

        

        Generation++;
    }

}
