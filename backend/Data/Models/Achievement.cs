namespace Backend.Data.Models;

public class Achievement
{
    public int Id { get; set; }
    public string Key { get; set; } = null!; 
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Trigger { get; set; } = null!;
    public int Threshold { get; set; }
}