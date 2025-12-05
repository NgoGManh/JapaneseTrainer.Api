using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JapaneseTrainer.Api.Migrations
{
    /// <inheritdoc />
    public partial class FeatureStudySrs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReviewSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CorrectCount = table.Column<int>(type: "int", nullable: false),
                    TotalAnswered = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudyProgresses",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Skill = table.Column<int>(type: "int", nullable: false),
                    Stage = table.Column<int>(type: "int", nullable: false),
                    LastReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextReviewAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CorrectStreak = table.Column<int>(type: "int", nullable: false),
                    WrongCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyProgresses", x => new { x.UserId, x.ItemId, x.Skill });
                    table.ForeignKey(
                        name: "FK_StudyProgresses_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDifficultItems",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDifficultItems", x => new { x.UserId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_UserDifficultItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudyProgresses_ItemId",
                table: "StudyProgresses",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDifficultItems_ItemId",
                table: "UserDifficultItems",
                column: "ItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewSessions");

            migrationBuilder.DropTable(
                name: "StudyProgresses");

            migrationBuilder.DropTable(
                name: "UserDifficultItems");
        }
    }
}
