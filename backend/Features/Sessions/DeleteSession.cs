using System.Security.Claims;
using Backend.Common.Endpoints;
using Backend.Common.Services;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Features.Sessions;

public static class DeleteSession
{
   public class Endpoint : IEndpoint
   {
      public void MapEndpoint(IEndpointRouteBuilder app)
      {
         app.MapDelete("sessions/{id}", Handler)
            .WithTags("Sessions")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
      }
   }

   public static async Task<IResult> Handler(int id, AppDbContext context, IPlayerService playerService, ClaimsPrincipal user)
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

      var session = await context.Sessions.FirstOrDefaultAsync(s => s.Id == id && s.PlayerId == player.Id);
      if (session is null)
      {
         return TypedResults.NotFound();
      }

      context.Sessions.Remove(session);
      await context.SaveChangesAsync();

      return TypedResults.NoContent();
   }
}