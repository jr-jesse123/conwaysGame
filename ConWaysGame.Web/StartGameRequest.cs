
/// <summary>
/// Request to get the next state of the game
/// </summary>
public record StartGameRequest(List<Coords> LiveCells, int GameLenght);
