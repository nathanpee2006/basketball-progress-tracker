using System.Security.Claims;
using Backend.Common.Endpoints;
using Backend.Common.Services;
using Backend.Data;
using Microsoft.AspNetCore.Http.HttpResults;

namespace backend.Features.Analytics;

public static class GetConsistency
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("analytics/consistency", Handler)
                .WithTags("Analytics")
                .Produces<ConsistencyDto>()
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization();
        }
    }

    public static async Task<Results<Ok<ConsistencyDto>, UnauthorizedHttpResult, NotFound>> Handler(AppDbContext context, IPlayerService playerService, IConsistencyService consistencyService, ClaimsPrincipal user, CancellationToken cancellationToken)
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

        return TypedResults.Ok(await consistencyService.GetConsistencyAsync(player.Id, cancellationToken));
    }
}