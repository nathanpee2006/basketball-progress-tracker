namespace Backend.Models
{
    public class Session
    {
        public int Id { get; set; }
        public string DrillType { get; set; } = "";
        public int ShotsMade { get; set; }
        public int ShotsAttempted { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
