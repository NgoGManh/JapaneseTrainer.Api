using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JapaneseTrainer.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Japanese = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Reading = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Romaji = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Meaning = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HashKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kanjis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Character = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Meaning = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Onyomi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Kunyomi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Strokes = table.Column<int>(type: "int", nullable: true),
                    Level = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kanjis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DictionaryEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Japanese = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Reading = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Romaji = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Meaning = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PartOfSpeech = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    JlptLevel = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    KanjiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DictionaryEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DictionaryEntries_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DictionaryEntries_Kanjis_KanjiId",
                        column: x => x.KanjiId,
                        principalTable: "Kanjis",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Audios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DictionaryEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Audios_DictionaryEntries_DictionaryEntryId",
                        column: x => x.DictionaryEntryId,
                        principalTable: "DictionaryEntries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Audios_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExampleSentences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Japanese = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Reading = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Romaji = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Meaning = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DictionaryEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExampleSentences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExampleSentences_DictionaryEntries_DictionaryEntryId",
                        column: x => x.DictionaryEntryId,
                        principalTable: "DictionaryEntries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExampleSentences_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Audios_DictionaryEntryId",
                table: "Audios",
                column: "DictionaryEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Audios_ItemId",
                table: "Audios",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryEntries_ItemId",
                table: "DictionaryEntries",
                column: "ItemId",
                unique: true,
                filter: "[ItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryEntries_KanjiId",
                table: "DictionaryEntries",
                column: "KanjiId");

            migrationBuilder.CreateIndex(
                name: "IX_ExampleSentences_DictionaryEntryId",
                table: "ExampleSentences",
                column: "DictionaryEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ExampleSentences_ItemId",
                table: "ExampleSentences",
                column: "ItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audios");

            migrationBuilder.DropTable(
                name: "ExampleSentences");

            migrationBuilder.DropTable(
                name: "DictionaryEntries");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Kanjis");
        }
    }
}
