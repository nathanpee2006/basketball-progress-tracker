namespace Backend.Data.Models
{
    public class Session
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;
        
        public DateOnly Date { get; set; }
        
        public int PaintMakes { get; set; }
        public int PaintAttempts { get; set; }
        
        public int MidrangeMakes { get; set; }
        public int MidrangeAttempts { get; set; }
        
        public int ThreePointMakes { get; set; }
        public int ThreePointAttempts { get; set; }
        
        public int FreeThrowMakes { get; set; }
        public int FreeThrowAttempts { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public ICollection<Drill> Drills { get; set; } = new List<Drill>();
    }
}
