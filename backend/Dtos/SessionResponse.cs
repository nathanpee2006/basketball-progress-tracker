namespace Backend.Dtos;

public record SessionResponse(
    int Id,
    DateOnly Date,
    int PaintMakes,
    int PaintAttempts,
    int MidrangeMakes,
    int MidrangeAttempts,
    int ThreePointMakes,
    int ThreePointAttempts,
    int FreeThrowMakes,
    int FreeThrowAttempts,
    DateTime CreatedAt
);
