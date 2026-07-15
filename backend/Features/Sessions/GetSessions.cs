using System.Security.Claims;
using Backend.Common.Endpoints;
using Backend.Common.Services;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Sessions;

public static class GetSessions
{
   public record Response(
   int Id,
   DateOnly Date,
   int PaintShotPercentage,
   int MidrangeShotPercentage,
   int ThreePointShotPercentage,
   int FreeThrowShotPercentage,
   int OverallShotPercentage
);

   public class Endpoint : IEndpoint
   {
      public void MapEndpoint(IEndpointRouteBuilder app)
      {
         app.MapGet("sessions", Handler).WithTags("Sessions").Produces<Response>().RequireAuthorization();
      }
   }

   public static async Task<IResult> Handler(AppDbContext context, IPlayerService playerService, ClaimsPrincipal user, CancellationToken cancellationToken)
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

      var sessions = await context.Sessions
               .AsNoTracking()
               .Where(s => s.PlayerId == player.Id)
               .OrderByDescending(s => s.Date)
               .ThenByDescending(s => s.CreatedAt)
               .Select(s => new Response(
                  s.Id,
                  s.Date,
                  PaintShotPercentage: s.PaintAttempts != 0 ? (int)Math.Round((double)s.PaintMakes / s.PaintAttempts * 100) : 0,
                  MidrangeShotPercentage: s.MidrangeAttempts != 0 ? (int)Math.Round((double)s.MidrangeMakes / s.MidrangeAttempts * 100) : 0,
                  ThreePointShotPercentage: s.ThreePointAttempts != 0 ? (int)Math.Round((double)s.ThreePointMakes / s.ThreePointAttempts * 100) : 0,
                  FreeThrowShotPercentage: s.FreeThrowAttempts != 0 ? (int)Math.Round((double)s.FreeThrowMakes / s.FreeThrowAttempts * 100) : 0,
                  OverallShotPercentage: (s.PaintAttempts + s.MidrangeAttempts + s.ThreePointAttempts + s.FreeThrowAttempts) != 0
                     ? (int)Math.Round((double)(s.PaintMakes + s.MidrangeMakes + s.ThreePointMakes + s.FreeThrowMakes)
                        / (s.PaintAttempts + s.MidrangeAttempts + s.ThreePointAttempts + s.FreeThrowAttempts) * 100)
                     : 0
               ))
               .ToListAsync(cancellationToken);

         return TypedResults.Ok(sessions);
   }
}