namespace ConWaysGame.Web.Dtos;
using ConWaysGame.Web.Dtos;

/// <summary>
/// Request to get the next state of the game
/// </summary>
public record StartGameRequest(List<Coords> LiveCells, int GameLenght);
