using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Backend.Data;
using System.Security.Claims;
using backend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Clerk:Authority"];
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Clerk:Issuer"],
            ValidAudience = builder.Configuration["Clerk:Audience"]
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactDevPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ISessionService, SessionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("ReactDevPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapGet("/api/sessions", async (HttpContext ctx, IPlayerService playerService, ISessionService sessionService) =>
{
    var clerkUserId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrWhiteSpace(clerkUserId))
    {
        return Results.Unauthorized();
    }

    var timeZoneHeader = ctx.Request.Headers["Time-Zone"].FirstOrDefault();
    var player = await playerService.GetOrCreateAsync(clerkUserId, timeZoneHeader);
    var sessions = await sessionService.ListByPlayerAsync(player.Id);

    return Results.Ok(sessions);
}).RequireAuthorization();

app.Run();