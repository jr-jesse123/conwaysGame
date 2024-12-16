/*
 Any live cell with fewer than two live neighbors dies, as if by underpopulation.
Any live cell with two or three live neighbors lives on to the next generation.
Any live cell with more than three live neighbors dies, as if by overpopulation.
Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction.
 */

//TODO: THINK OF SPARSE AND DENSE GRIDS
//TODO: THINK AND TEST PARALLELISM, specially for final state we can share the state for trheads, 

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
