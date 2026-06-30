using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerSessionsAndDrills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DrillType",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "ShotsMade",
                table: "Sessions",
                newName: "ThreePointMakes");

            migrationBuilder.RenameColumn(
                name: "ShotsAttempted",
                table: "Sessions",
                newName: "ThreePointAttempts");

            migrationBuilder.RenameColumn(
                name: "DurationMinutes",
                table: "Sessions",
                newName: "PlayerId");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "Sessions",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Sessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "FreeThrowAttempts",
                table: "Sessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FreeThrowMakes",
                table: "Sessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MidrangeAttempts",
                table: "Sessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MidrangeMakes",
                table: "Sessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaintAttempts",
                table: "Sessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaintMakes",
                table: "Sessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Drills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SessionId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CompletionTimeInSeconds = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Drills_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClerkUserId = table.Column<string>(type: "text", nullable: false),
                    TimeZone = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_PlayerId_Date",
                table: "Sessions",
                columns: new[] { "PlayerId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Drills_SessionId",
                table: "Drills",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_ClerkUserId",
                table: "Players",
                column: "ClerkUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Players_PlayerId",
                table: "Sessions",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Players_PlayerId",
                table: "Sessions");

            migrationBuilder.DropTable(
                name: "Drills");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_PlayerId_Date",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "FreeThrowAttempts",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "FreeThrowMakes",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "MidrangeAttempts",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "MidrangeMakes",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "PaintAttempts",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "PaintMakes",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "ThreePointMakes",
                table: "Sessions",
                newName: "ShotsMade");

            migrationBuilder.RenameColumn(
                name: "ThreePointAttempts",
                table: "Sessions",
                newName: "ShotsAttempted");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "Sessions",
                newName: "DurationMinutes");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Sessions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "DrillType",
                table: "Sessions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
