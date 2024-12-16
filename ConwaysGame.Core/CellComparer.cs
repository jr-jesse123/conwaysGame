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
