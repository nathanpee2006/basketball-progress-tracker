namespace Backend.Data.Models;

public class PlayerAchievement
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public int AchievementId { get; set; }
    public DateTime AchievedAt { get; set; }
}