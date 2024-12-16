
/// <summary>
/// Request to get the next state of the game
/// </summary>
/// <param name="Id">The id of the game</param>
/// <param name="NumberOfGenerations">Number of Generations to process</param>
public record NextStateRequest(int Id, int NumberOfGenerations = int.MaxValue);
