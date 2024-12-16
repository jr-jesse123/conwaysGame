

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

public class CellComparer : IComparer<(int x, int y)>
{
    public int Compare((int x, int y) left, (int x, int y) right)
    {
        if (left.x == right.x)
        {
            return left.y.CompareTo(right.y);
        }
        return left.x.CompareTo(right.x);

    }
}

//public static class GameBoardExtensions
//{
//    //TODO: DOCUEMNTAR
//    public static byte LookFoward(this ref Gameboard board, int idx, (int x, int y) targetValue)
//    {
//        var (baseX, baseY) = board[idx];
//        for (int i = idx + 1; i < board.Length; i++)
//        {
//            var (currentX, currentY) = board[i];

//            if (currentX == baseX && currentY == baseY)
//            {
//                return 0b0000_0001;
//            }

//            if (currentX > targetValue.x || currentY > targetValue.y)
//            {
//                break;
//            }
//        }

//        return 0b0000_0000;
//    }


//    public static byte LookBackWard(this ref Gameboard board, int idx, (int x, int y) targetValue)
//    {
//        var (baseX, baseY) = board[idx];
//        for (int i = idx + 1; i >= 0; i--)
//        {
//            var (currentX, currentY) = board[i];

//            if (currentX == baseX && currentY == baseY)
//            {
//                return 0b0000_0001;
//            }

//            if (currentX < targetValue.x || currentY < targetValue.y)
//            {
//                break;
//            }
//        }

//        return 0b0000_0000;
//    }


//    public static Gameboard GetWhileXNotChangedAsc(this ref Gameboard board, int idx)
//    {
//        var (initialX, _) = board[idx];
//        for (int delta = idx; delta < board.Length; delta++)
//        {
//            var (_x, _y) = board[delta];

//            if (_x != initialX)
//            {
//                return board.Slice(idx, delta - idx);
//            }
//        }

//        return Gameboard.Empty;
//    }

//    public static Gameboard GetWhileXNotChangedDesc(this ref Gameboard board, int idx)
//    {
//        var (initicalX, _) = board[idx];

//        for (int delta = idx; delta >= 0; delta--)
//        {
//            var (_x, _y) = board[delta];
//            if (_x != initicalX)
//            {
//                return board.Slice(delta, idx - delta);
//            }
//        }
//        return Gameboard.Empty;

//    } 

//    //public static Gameboard GetUpperLine(this ref Gameboard board, int idx)
//    //{
//    //    var (x, y) = board[idx];
//    //    for (int lineEnd = idx; lineEnd >= 0; lineEnd--)
//    //    {
//    //        var (_x, _y) = board[lineEnd];
//    //        if (_x == x - 1)
//    //        {
//    //            return board.GetWhileXNotChangedDesc(lineEnd);
//    //        }
//    //        if (_x < x - 1)
//    //        {
//    //            return Gameboard.Empty;
//    //        }
//    //    }
//    //    return Gameboard.Empty;

//    //}

//    //public static Gameboard GetLowerLine(this ref Gameboard board, int idx)
//    //{
//    //    var (x, y) = board[idx];
//    //    for (int lineEnd = idx; lineEnd < board.Length; lineEnd++)
//    //    {
//    //        var (_x, _y) = board[lineEnd];
//    //        if (_x == x + 1)
//    //        {
//    //            return board.GetWhileXNotChangedAsc(lineEnd);
//    //        }
//    //        if (_x > x + 1)
//    //        {
//    //            return Gameboard.Empty;
//    //        }
//    //    }
//    //    return Gameboard.Empty;
//    //}
//}


    public ref struct Game
    {
    private readonly int _gridSideLenght;
    //private Span<(int x, int y)> _board;
    private List<(int x, int y)> _board;

    ArrayPool<(int, int)> arrayPool = ArrayPool<(int,int)>.Shared;
        //public readonly ReadOnlySpan<(int x, int y)> Board { get => _board;  }
        public readonly List<(int x, int y)> Board { get => _board; }
    public int Generation { get; private set; } = 0;

        public bool HasStabilized { get; private set; } = false;

        private int MaxGenerations { get; init ; } = 1000;

    public Game()
    {
        throw new NotSupportedException("You must provide a board to start the game.");
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
        //_gridLenght = (int)Math.Sqrt(board.Length) ;


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
        var positionsWithLiveNeihbors = new Dictionary<(int, int), int>();

        for (int i = 0; i < Board.Count; i++)
        {
            AddNeighbors(i, positionsWithLiveNeihbors);
        }

        //Span<(int,int)> newLiveCells = stackalloc (int x, int y)[_gridSideLenght ^ 2];

        var internalArray = arrayPool.Rent(_gridSideLenght);
        var newLiveCells = new List<(int x, int y)>(internalArray);
        newLiveCells.Clear();
        foreach (var (x, y) in positionsWithLiveNeihbors.Keys)
        {
            if(x < 0 || y < 0 || x > _gridSideLenght || y > _gridSideLenght)
                continue;

            var liveNeighbors = positionsWithLiveNeihbors[(x, y)];
            if (liveNeighbors == 2 || liveNeighbors == 3)
            {
                //newLiveCells[currentIdx++] = (x, y);
                newLiveCells.Add((x, y));
            }
            else if (liveNeighbors == 3 && Board.Contains((x,y))) //TODO: MAYBE USE BINARY OR STORE THE POSITIONS IN A HASHSET
            {
                //newLiveCells[currentIdx++] = (x, y); //TODO: CHECAR PARA O VOLUME DE CÓPIAS DO VALOR DE TUPLAS.
                newLiveCells.Add((x, y));
            }
        }

        if (newLiveCells.SequenceEqual(Board))
        {
            HasStabilized = true;
        }

        _board.Clear();
        _board.AddRange(newLiveCells);  

        arrayPool.Return(internalArray);

        Generation++;
    }

}
