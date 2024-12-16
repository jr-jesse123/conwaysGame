

/*
 Any live cell with fewer than two live neighbors dies, as if by underpopulation.
Any live cell with two or three live neighbors lives on to the next generation.
Any live cell with more than three live neighbors dies, as if by overpopulation.
Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction.
 */

//TODO: THINK OF SPARSE AND DENSE GRIDS
//TODO: THINK AND TEST PARALLELISM, specially for final state we can share the state for trheads, 

global using GameBoard = bool[,];
global using LivePopulation = (int x, int y)[];
global using Gameboard3 = System.Span<(int x, int y)>;
namespace ConwaysGame.Core;

public ref struct Position
{
    public int X { get; set; }
    public int Y { get; set; }
    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    public class CellComparable : IComparer<(int x, int y)>
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

public static class GameBoardExtensions
{
    public static Gameboard3 GetWhileXNotChangedAsc(this ref Gameboard3 board, int idx)
    {
        var (initialX, _) = board[idx];
        for (int delta = idx; delta < board.Length; delta++)
        {
            var (_x, _y) = board[delta];

            if (_x != initialX)
            {
                return board.Slice(idx, delta - idx);
            }
        }

        return Gameboard3.Empty;
    }

    public static Gameboard3 GetWhileXNotChangedDesc(this ref Gameboard3 board, int idx)
    {
        var (initicalX, _) = board[idx];

        for (int delta = idx; delta >= 0; delta--)
        {
            var (_x, _y) = board[delta];
            if (_x != initicalX)
            {
                return board.Slice(delta, idx - delta);
            }
        }
        return Gameboard3.Empty;

    } 

    public static Gameboard3 GetUpperLine(this ref Gameboard3 board, int idx)
    {
        var (x, y) = board[idx];
        for (int lineEnd = idx; lineEnd >= 0; lineEnd--)
        {
            var (_x, _y) = board[lineEnd];
            if (_x == x - 1)
            {
                return board.GetWhileXNotChangedDesc(lineEnd);
            }
            if (_x < x - 1)
            {
                return Gameboard3.Empty;
            }
        }
        return Gameboard3.Empty;

    }

    public static Gameboard3 GetLowerLine(this ref Gameboard3 board, int idx)
    {
        var (x, y) = board[idx];
        for (int lineEnd = idx; lineEnd < board.Length; lineEnd++)
        {
            var (_x, _y) = board[lineEnd];
            if (_x == x + 1)
            {
                return board.GetWhileXNotChangedAsc(lineEnd);
            }
            if (_x > x + 1)
            {
                return Gameboard3.Empty;
            }
        }
        return Gameboard3.Empty;
    }
}


    public ref struct GameBoard2
    {
        private readonly int _gridLenght;
        private Span<(int x, int y)> _board;

        public readonly Span<(int x, int y)> Board { get => _board; }

    //public void NextGeneration()
    //{
    //    var newBoard = new (int x, int y)[_board.Length];
    //    Span<(int x, int y)> newBoardSpan = newBoard;

    //    for (int i = 0; i < _board.Length; i++)
    //    {
    //        var cell = _board[i];
    //        int liveNeighbors = GetLiveNeighBors(cell.x, cell.y);

    //        if (liveNeighbors == 2 || liveNeighbors == 3)
    //        {
    //            newBoardSpan[i] = cell;
    //        }
    //        else if (liveNeighbors == 3)
    //        {
    //            newBoardSpan[i] = cell;
    //        }
    //    }

    //    _board = newBoardSpan;
    //}


    public GameBoard2(Gameboard3 board)
    {
        
        board.Sort(new CellComparable());

        if (Math.Sqrt(board.Length) % 1 != 0)
        {
            throw new ArgumentException("The board length must be a perfect square.");
        }

        _board = board;
        _gridLenght = (int)Math.Sqrt(board.Length) ;

    }

    public enum NeighBorPosition
    { 
        Left,
        Right,
        Upper,
        Lower,
        UpperLeft,
        UpperRight,
        LowerLeft,
        LowerRight
    }


    private static (int, int)? IsNeighborAlive(Gameboard3 board, int currentIdx, NeighBorPosition position)
    {
        var (currentX, currentY) = board[currentIdx];

        var (newX, newY) = position switch
        {
            NeighBorPosition.Left => (currentX, currentY - 1),
            NeighBorPosition.Right => (currentX, currentY + 1),
            NeighBorPosition.Upper => (currentX - 1, currentY),
            NeighBorPosition.Lower => (currentX + 1, currentY),
            NeighBorPosition.UpperLeft => (currentX - 1, currentY - 1),
            NeighBorPosition.UpperRight => (currentX - 1, currentY + 1),
            NeighBorPosition.LowerLeft => (currentX + 1, currentY - 1),
            NeighBorPosition.LowerRight => (currentX + 1, currentY + 1),
            _ => throw new ArgumentException("Invalid position")
        };

        board.BinarySearch(new CellComparable());

    }

    //TODO: FAZER INTERNO
    //TODO: TENTAR OPTIMIZAR
    public int GetLiveNeighBors(int idx)
        {
        var upperLine = _board.GetUpperLine(idx);
        var lowerLine = _board.GetLowerLine(idx);

        (int , int)? NearestleftNeigbor = idx - 1 >= 0 ? _board[idx - 1] : null;

        (int, int)? nearestRightNeigbor = idx + 1 < _board.Length ? _board[idx + 1] : null;

        _board





            for (sbyte horizontalDelta = -1; horizontalDelta <= 1; horizontalDelta++)
        {
            for (sbyte verticalDelta = -1; verticalDelta <= 1; verticalDelta++)
            {
                if (horizontalDelta == 0 && verticalDelta == 0) continue; //current cell

                int horizontalShift = x + horizontalDelta;
                int verticalShift = y + verticalDelta;
                var targetIndex = horizontalShift * _gridLenght + verticalShift;

                if (horizontalShift >= 0 && horizontalShift < _gridLenght &&
                    verticalShift >= 0 && verticalShift < _gridLenght &&
                    _board[targetIndex])
                {
                    count++;
                }

            }
        }
        return count;
    }





    //public GameBoard2(Span<bool> board)
    //{
    //    if (Math.Sqrt(board.Length) % 1 != 0)
    //    {
    //        throw new ArgumentException("The board length must be a perfect square.");
    //    }
    //    _board = board;
    //    _gridLenght = (int)Math.Sqrt(board.Length);
    //}



}

public class GameBoardModule
    {


        public static GameBoard Populate(GameBoard board, LivePopulation population)
        {
            foreach (var (x, y) in population)
            {
                board[x, y] = true;
            }
            return board;
        }


        public static int GetLiveNeighbors(GameBoard board, int x, int y)
        {
            int count = 0;
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    int x1 = x + i;
                    int y1 = y + j;
                    if (x1 >= 0 && x1 < rows && y1 >= 0 && y1 < cols && board[x1, y1])
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public static void PrintBoard(bool[,] board)
        {

            int rows = board.GetLength(0);
            int cols = board.GetLength(1);

            Console.WriteLine("--------------------------------------------------------------------------");
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
                {
                    Console.Write(board[x, y] ? "O" : ".");
                }
                Console.WriteLine();
            }
        }
    }


