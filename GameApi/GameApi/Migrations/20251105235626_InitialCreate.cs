using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GameApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerScores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Score = table.Column<int>(type: "INTEGER", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerScores", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "PlayerScores",
                columns: new[] { "Id", "CreatedAt", "Level", "PlayerName", "Score" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 11, 5, 23, 56, 24, 575, DateTimeKind.Utc).AddTicks(5960), 5, "Andi", 1200 },
                    { 2, new DateTime(2025, 11, 5, 23, 56, 24, 575, DateTimeKind.Utc).AddTicks(6629), 4, "Nadia", 980 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerScores_Score_Level_Id",
                table: "PlayerScores",
                columns: new[] { "Score", "Level", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerScores");
        }
    }
}
