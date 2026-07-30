namespace Backend.Data.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string ClerkUserId { get; set; } = string.Empty; 
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } = string.Empty;
        public string TimeZone { get; set; } = "UTC";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
