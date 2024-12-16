namespace ConWaysGame.Web.Dtos;
/// <summary>
/// Error Details
/// </summary>
public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string? Details { get; set; } // Opcional, pode incluir informações adicionais

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}
