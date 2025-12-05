using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JapaneseTrainer.Api.Migrations
{
    /// <inheritdoc />
    public partial class FeatureExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Skill = table.Column<int>(type: "int", nullable: false),
                    Prompt = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    OptionsJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Answer = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GrammarMasterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GrammarPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercises_GrammarMasters_GrammarMasterId",
                        column: x => x.GrammarMasterId,
                        principalTable: "GrammarMasters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Exercises_GrammarPackages_GrammarPackageId",
                        column: x => x.GrammarPackageId,
                        principalTable: "GrammarPackages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Exercises_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_GrammarMasterId",
                table: "Exercises",
                column: "GrammarMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_GrammarPackageId",
                table: "Exercises",
                column: "GrammarPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_ItemId",
                table: "Exercises",
                column: "ItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exercises");
        }
    }
}
