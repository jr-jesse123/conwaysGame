

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

public struct Position : IComparable<Position>
{
    public int X { get; set; }
    public int Y { get; set; }
    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int CompareTo(Position other)
    {
        return X.CompareTo(other.X) == 0 ? Y.CompareTo(other.Y) : X.CompareTo(other.X);
    }

    public override bool Equals(object obj)
    {
        if (obj is Position other)
        {
            return X == other.X && Y == other.Y;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode();
    }

    public static bool operator ==(Position left, Position right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Position left, Position right)
    {
        return !(left == right);
    }

    public static bool operator <(Position left, Position right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Position left, Position right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Position left, Position right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Position left, Position right)
    {
        return left.CompareTo(right) >= 0;
    }
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
    //TODO: DOCUEMNTAR
    public static byte LookFoward(this ref Gameboard3 board, int idx, (int x, int y) targetValue)
    {
        var (baseX, baseY) = board[idx];
        for (int i = idx + 1; i < board.Length; i++)
        {
            var (currentX, currentY) = board[i];

            if (currentX == baseX && currentY == baseY)
            {
                return 0b0000_0001;
            }

            if (currentX > targetValue.x || currentY > targetValue.y)
            {
                break;
            }
        }

        return 0b0000_0000;
    }


    public static byte LookBackWard(this ref Gameboard3 board, int idx, (int x, int y) targetValue)
    {
        var (baseX, baseY) = board[idx];
        for (int i = idx + 1; i >= 0; i--)
        {
            var (currentX, currentY) = board[i];

            if (currentX == baseX && currentY == baseY)
            {
                return 0b0000_0001;
            }

            if (currentX < targetValue.x || currentY < targetValue.y)
            {
                break;
            }
        }

        return 0b0000_0000;
    }


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



    //TODO: FAZER INTERNO
    //TODO: TENTAR OPTIMIZAR
    public int GetLiveNeighBors(int idx)
    {
        

        var (x, y) = _board[idx];

        return 
            _board.LookBackWard(idx, (x - 1, y)) +
            _board.LookFoward(idx, (x + 1, y)) +

            _board.LookFoward(idx, (x, y - 1) ) +
            _board.LookFoward(idx, (x, y + 1)) +
            _board.LookBackWard(0, (x - 1, y - 1)) +

            _board.LookFoward(0, (x - 1, y + 1)) +

            _board.LookBackWard(0, (x + 1, y - 1)) +
            _board.LookFoward(0, (x + 1, y + 1));

    }


    /*
     Any live cell with fewer than two live neighbors dies, as if by underpopulation.
    Any live cell with two or three live neighbors lives on to the next generation.
    Any live cell with more than three live neighbors dies, as if by overpopulation.
    Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction.
     */


    public void AddNeighbors(int idx, Dictionary<(int,int), int> acc)
    {
        Span<(int x, int y)?> neighbors = stackalloc  (int x, int y)?[8];
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
            if (acc.ContainsKey((x, y)))
            {
                acc[(x, y)]++;
            }
            else
            {
                acc[(x, y)] = 1;
            } 
        }

    }


    public void AdvanceGeneration()
    {
        var positionsWithLiveNeihbors = new Dictionary<(int, int), int>();

        for (int i = 0; i < Board.Length; i++)
        {
            AddNeighbors(i, positionsWithLiveNeihbors);
        }

        Span<(int,int)> newLiveCells = stackalloc (int x, int y)[ _gridLenght * _gridLenght];
        var currentIdx = 0;
        foreach (var (x, y) in positionsWithLiveNeihbors.Keys)
        {
            if(x < 0 || y < 0 || x > _gridLenght * _gridLenght || y > _gridLenght * _gridLenght)
                continue;

            var liveNeighbors = positionsWithLiveNeihbors[(x, y)];
            if (liveNeighbors == 2 || liveNeighbors == 3)
            {
                newLiveCells[currentIdx++] = (x, y);
            }
            else if (liveNeighbors == 3 && Board.Contains((x,y))) //TODO: MAYBE USE BINARY OR STORE THE POSITIONS IN A HASHSET
            {
                newLiveCells[currentIdx++] = (x, y); //TODO: CHECAR PARA O VOLUME DE CÓPIAS DO VALOR DE TUPLAS.
            }
        }

        newLiveCells.CopyTo(_board);

    }




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


