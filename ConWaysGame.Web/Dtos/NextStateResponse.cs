namespace ConWaysGame.Web.Dtos;
using ConWaysGame.Web.Dtos;

/// <summary>
/// Request to get the next state of the game
/// </summary>
public record NextStateResponse(int Id, int CurrentGeneration, List<Coords> LiveCells);
