namespace Backend.Models;

public class Drill
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public Session Session { get; set; } = null!;

    public string Name { get; set; } = string.Empty; 
    public int CompletionTimeInSeconds { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}