using System.Security.Claims;
using Backend.Common.Endpoints;
using Backend.Common.Services;
using Backend.Data;
using Backend.Data.Models;
using FluentValidation;

namespace Backend.Features.Sessions;

public static class CreateSession
{
    public record Request(
       DateOnly Date,
       int PaintMakes,
       int PaintAttempts,
       int MidrangeMakes,
       int MidrangeAttempts,
       int ThreePointMakes,
       int ThreePointAttempts,
       int FreeThrowMakes,
       int FreeThrowAttempts,
       List<DrillRequest> Drills
   );

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

    public record DrillRequest(string Name, int CompletionTimeInSeconds);
    public record DrillResponse(int Id, string Name, int CompletionTimeInSeconds);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Date).NotEmpty().LessThan(DateOnly.FromDateTime(DateTime.Today.AddDays(1))); // Note: exception gets thrown when date is empty string
            RuleFor(r => r.PaintMakes).GreaterThanOrEqualTo(0).LessThanOrEqualTo(r => r.PaintAttempts);
            RuleFor(r => r.PaintAttempts).GreaterThanOrEqualTo(0);
            RuleFor(r => r.MidrangeMakes).GreaterThanOrEqualTo(0).LessThanOrEqualTo(r => r.MidrangeAttempts);
            RuleFor(r => r.MidrangeAttempts).GreaterThanOrEqualTo(0);
            RuleFor(r => r.ThreePointMakes).GreaterThanOrEqualTo(0).LessThanOrEqualTo(r => r.ThreePointAttempts);
            RuleFor(r => r.ThreePointAttempts).GreaterThanOrEqualTo(0);
            RuleFor(r => r.FreeThrowMakes).GreaterThanOrEqualTo(0).LessThanOrEqualTo(r => r.FreeThrowAttempts);
            RuleFor(r => r.FreeThrowAttempts).GreaterThanOrEqualTo(0);
            RuleFor(r => r.Drills).NotNull();
            RuleForEach(r => r.Drills).SetValidator(new DrillValidator());
        }
    }

    public sealed class DrillValidator : AbstractValidator<DrillRequest>
    {
        public DrillValidator()
        {
            RuleFor(d => d.Name).NotEmpty();
            RuleFor(d => d.CompletionTimeInSeconds).GreaterThan(0);
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("sessions", Handler)
                .WithTags("Sessions")
                .Produces<Response>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(Request request, AppDbContext context, IPlayerService playerService, ClaimsPrincipal user, IValidator<Request> validator)
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

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return TypedResults.BadRequest(validationResult.Errors);
        }

        var session = new Session
        {
            PlayerId = player.Id,
            Date = request.Date,
            PaintMakes = request.PaintMakes,
            PaintAttempts = request.PaintAttempts,
            MidrangeMakes = request.MidrangeMakes,
            MidrangeAttempts = request.MidrangeAttempts,
            ThreePointMakes = request.ThreePointMakes,
            ThreePointAttempts = request.ThreePointAttempts,
            FreeThrowMakes = request.FreeThrowMakes,
            FreeThrowAttempts = request.FreeThrowAttempts,
            Drills = [.. request.Drills.Select(d => new Drill
            {
                Name = d.Name,
                CompletionTimeInSeconds = d.CompletionTimeInSeconds
            })]
        };

        context.Sessions.Add(session);
        await context.SaveChangesAsync();

        var overallMakes = session.PaintMakes + session.MidrangeMakes + session.ThreePointMakes + session.FreeThrowMakes;
        var overallAttempts = session.PaintAttempts + session.MidrangeAttempts + session.ThreePointAttempts + session.FreeThrowAttempts;

        return TypedResults.Created($"/api/sessions/{session.Id}", new Response(
            session.Id,
            session.Date,
            session.PaintMakes,
            session.PaintAttempts,
            session.MidrangeMakes,
            session.MidrangeAttempts,
            session.ThreePointMakes,
            session.ThreePointAttempts,
            session.FreeThrowMakes,
            session.FreeThrowAttempts,
            overallMakes,
            overallAttempts,
            PaintShotPercentage: session.PaintAttempts != 0 ? (int)Math.Round((double)session.PaintMakes / session.PaintAttempts * 100) : 0,
            MidrangeShotPercentage: session.MidrangeAttempts != 0 ? (int)Math.Round((double)session.MidrangeMakes / session.MidrangeAttempts * 100) : 0,
            ThreePointShotPercentage: session.ThreePointAttempts != 0 ? (int)Math.Round((double)session.ThreePointMakes / session.ThreePointAttempts * 100) : 0,
            FreeThrowShotPercentage: session.FreeThrowAttempts != 0 ? (int)Math.Round((double)session.FreeThrowMakes / session.FreeThrowAttempts * 100) : 0,
            OverallShotPercentage: (session.PaintAttempts + session.MidrangeAttempts + session.ThreePointAttempts + session.FreeThrowAttempts) != 0
                ? (int)Math.Round((double)(session.PaintMakes + session.MidrangeMakes + session.ThreePointMakes + session.FreeThrowMakes) / (session.PaintAttempts + session.MidrangeAttempts + session.ThreePointAttempts + session.FreeThrowAttempts) * 100)
                : 0,
            [.. session.Drills.Select(d => new DrillResponse(d.Id, d.Name, d.CompletionTimeInSeconds))]
        ));
    }
}