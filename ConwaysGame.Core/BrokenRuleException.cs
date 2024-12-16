

/*
 Any live cell with fewer than two live neighbors dies, as if by underpopulation.
Any live cell with two or three live neighbors lives on to the next generation.
Any live cell with more than three live neighbors dies, as if by overpopulation.
Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction.
 */

//TODO: THINK OF SPARSE AND DENSE GRIDS
//TODO: THINK AND TEST PARALLELISM, specially for final state we can share the state for trheads, 




using System.Runtime.Serialization;

namespace ConwaysGame.Core;

public class BrokenRuleException : Exception
{
    public BrokenRuleException(string? message, Exception? innerException = null) : base(message, innerException)
    {
    }

    protected BrokenRuleException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
