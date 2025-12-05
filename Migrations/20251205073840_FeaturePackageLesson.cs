using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JapaneseTrainer.Api.Migrations
{
    /// <inheritdoc />
    public partial class FeaturePackageLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lessons_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonGrammars",
                columns: table => new
                {
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrammarMasterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrammarPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonGrammars", x => new { x.LessonId, x.GrammarMasterId });
                    table.ForeignKey(
                        name: "FK_LessonGrammars_GrammarMasters_GrammarMasterId",
                        column: x => x.GrammarMasterId,
                        principalTable: "GrammarMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonGrammars_GrammarPackages_GrammarPackageId",
                        column: x => x.GrammarPackageId,
                        principalTable: "GrammarPackages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LessonGrammars_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonItems",
                columns: table => new
                {
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonItems", x => new { x.LessonId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_LessonItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonItems_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonGrammars_GrammarMasterId",
                table: "LessonGrammars",
                column: "GrammarMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonGrammars_GrammarPackageId",
                table: "LessonGrammars",
                column: "GrammarPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonItems_ItemId",
                table: "LessonItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_PackageId",
                table: "Lessons",
                column: "PackageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LessonGrammars");

            migrationBuilder.DropTable(
                name: "LessonItems");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "Packages");
        }
    }
}
