using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JapaneseTrainer.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudyProgressForKanji : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyProgresses_Items_ItemId",
                table: "StudyProgresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyProgresses",
                table: "StudyProgresses");

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "StudyProgresses",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            // Add Id column as nullable first
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "StudyProgresses",
                type: "uniqueidentifier",
                nullable: true);

            // Generate unique Guid for existing records
            migrationBuilder.Sql(@"
                UPDATE StudyProgresses 
                SET Id = NEWID() 
                WHERE Id IS NULL
            ");

            // Make Id NOT NULL after populating values
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "StudyProgresses",
                type: "uniqueidentifier",
                nullable: false);

            migrationBuilder.AddColumn<Guid>(
                name: "KanjiId",
                table: "StudyProgresses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyProgresses",
                table: "StudyProgresses",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "LessonKanjis",
                columns: table => new
                {
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KanjiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonKanjis", x => new { x.LessonId, x.KanjiId });
                    table.ForeignKey(
                        name: "FK_LessonKanjis_Kanjis_KanjiId",
                        column: x => x.KanjiId,
                        principalTable: "Kanjis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonKanjis_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudyProgress_User_Item_Skill",
                table: "StudyProgresses",
                columns: new[] { "UserId", "ItemId", "Skill" },
                unique: true,
                filter: "[ItemId] IS NOT NULL AND [KanjiId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StudyProgress_User_Kanji_Skill",
                table: "StudyProgresses",
                columns: new[] { "UserId", "KanjiId", "Skill" },
                unique: true,
                filter: "[KanjiId] IS NOT NULL AND [ItemId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StudyProgresses_KanjiId",
                table: "StudyProgresses",
                column: "KanjiId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_StudyProgress_ItemOrKanji",
                table: "StudyProgresses",
                sql: "([ItemId] IS NOT NULL AND [KanjiId] IS NULL) OR ([ItemId] IS NULL AND [KanjiId] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_LessonKanjis_KanjiId",
                table: "LessonKanjis",
                column: "KanjiId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyProgresses_Items_ItemId",
                table: "StudyProgresses",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyProgresses_Kanjis_KanjiId",
                table: "StudyProgresses",
                column: "KanjiId",
                principalTable: "Kanjis",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyProgresses_Items_ItemId",
                table: "StudyProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyProgresses_Kanjis_KanjiId",
                table: "StudyProgresses");

            migrationBuilder.DropTable(
                name: "LessonKanjis");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyProgresses",
                table: "StudyProgresses");

            migrationBuilder.DropIndex(
                name: "IX_StudyProgress_User_Item_Skill",
                table: "StudyProgresses");

            migrationBuilder.DropIndex(
                name: "IX_StudyProgress_User_Kanji_Skill",
                table: "StudyProgresses");

            migrationBuilder.DropIndex(
                name: "IX_StudyProgresses_KanjiId",
                table: "StudyProgresses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_StudyProgress_ItemOrKanji",
                table: "StudyProgresses");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "StudyProgresses");

            migrationBuilder.DropColumn(
                name: "KanjiId",
                table: "StudyProgresses");

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "StudyProgresses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyProgresses",
                table: "StudyProgresses",
                columns: new[] { "UserId", "ItemId", "Skill" });

            migrationBuilder.AddForeignKey(
                name: "FK_StudyProgresses_Items_ItemId",
                table: "StudyProgresses",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
