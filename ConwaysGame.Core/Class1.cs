

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

namespace ConwaysGame.Core;
public class GameBoardModule
{
    public static GameBoard Create(int x, int y) => new bool[x, y];
    
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

