using System.Security.Claims;
using Backend.Common.Endpoints;
using Backend.Common.Services;
using Backend.Data;
using Backend.Data.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Sessions;

public class GetSession
{
    public record Response(
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
        int OverallMakes,
        int OverallAttempts,
        int PaintShotPercentage,
        int MidrangeShotPercentage,
        int ThreePointShotPercentage,
        int FreeThrowShotPercentage,
        int OverallShotPercentage,
        List<DrillResponse> Drills
    );
    public record DrillResponse(int Id, string Name, int CompletionTimeInSeconds);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("sessions/{id}", Handler)
                .WithTags("Sessions")
                .Produces<Response>()
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization();
        }
    }

    public static async Task<Results<Ok<Response>, NotFound, UnauthorizedHttpResult>> Handler(int id, AppDbContext context, IPlayerService playerService, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        var clerkUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (clerkUserId is null)
        {
            return TypedResults.Unauthorized();
        }

        var player = await playerService.GetByClerkUserIdAsync(clerkUserId);
        if (player is null)
        {
            return TypedResults.NotFound();
        }

        var session = await context.Sessions
            .AsNoTracking()
            .Where(s => s.Id == id && s.PlayerId == player.Id)
            .Select(s => new Response(
                s.Id, 
                s.Date, 
                s.PaintMakes, 
                s.PaintAttempts, 
                s.MidrangeMakes, 
                s.MidrangeAttempts, 
                s.ThreePointMakes, 
                s.ThreePointAttempts, 
                s.FreeThrowMakes, 
                s.FreeThrowAttempts, 
                s.PaintMakes + s.MidrangeMakes + s.ThreePointMakes + s.FreeThrowMakes, 
                s.PaintAttempts + s.MidrangeAttempts + s.ThreePointAttempts + s.FreeThrowAttempts,
                PaintShotPercentage: s.PaintAttempts != 0 ? (int)Math.Round((double)s.PaintMakes / s.PaintAttempts * 100) : 0,
                MidrangeShotPercentage: s.MidrangeAttempts != 0 ? (int)Math.Round((double)s.MidrangeMakes / s.MidrangeAttempts * 100) : 0,
                ThreePointShotPercentage: s.ThreePointAttempts != 0 ? (int)Math.Round((double)s.ThreePointMakes / s.ThreePointAttempts * 100) : 0,
                FreeThrowShotPercentage: s.FreeThrowAttempts != 0 ? (int)Math.Round((double)s.FreeThrowMakes / s.FreeThrowAttempts * 100) : 0,
                OverallShotPercentage: (s.PaintAttempts + s.MidrangeAttempts + s.ThreePointAttempts + s.FreeThrowAttempts) != 0
                    ? (int)Math.Round((double)(s.PaintMakes + s.MidrangeMakes + s.ThreePointMakes + s.FreeThrowMakes) / (s.PaintAttempts + s.MidrangeAttempts + s.ThreePointAttempts + s.FreeThrowAttempts) * 100)
                    : 0,
                s.Drills.Select(d => new DrillResponse(d.Id, d.Name, d.CompletionTimeInSeconds)).ToList()))
            .FirstOrDefaultAsync(cancellationToken);

        if (session is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(session);
    }
}