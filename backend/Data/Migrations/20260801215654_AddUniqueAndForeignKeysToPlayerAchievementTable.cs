using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueAndForeignKeysToPlayerAchievementTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PlayerAchievements_AchievementId",
                table: "PlayerAchievements",
                column: "AchievementId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAchievements_PlayerId_AchievementId",
                table: "PlayerAchievements",
                columns: new[] { "PlayerId", "AchievementId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerAchievements_Achievements_AchievementId",
                table: "PlayerAchievements",
                column: "AchievementId",
                principalTable: "Achievements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerAchievements_Players_PlayerId",
                table: "PlayerAchievements",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerAchievements_Achievements_AchievementId",
                table: "PlayerAchievements");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerAchievements_Players_PlayerId",
                table: "PlayerAchievements");

            migrationBuilder.DropIndex(
                name: "IX_PlayerAchievements_AchievementId",
                table: "PlayerAchievements");

            migrationBuilder.DropIndex(
                name: "IX_PlayerAchievements_PlayerId_AchievementId",
                table: "PlayerAchievements");
        }
    }
}
