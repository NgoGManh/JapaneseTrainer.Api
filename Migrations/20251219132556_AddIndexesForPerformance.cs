using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JapaneseTrainer.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesForPerformance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Kanjis_Character",
                table: "Kanjis",
                column: "Character",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kanjis_Level",
                table: "Kanjis",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CreatedAt",
                table: "Items",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Items_HashKey",
                table: "Items",
                column: "HashKey",
                unique: true,
                filter: "[HashKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Japanese_Reading",
                table: "Items",
                columns: new[] { "Japanese", "Reading" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_Type",
                table: "Items",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryEntries_CreatedAt",
                table: "DictionaryEntries",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryEntries_JlptLevel",
                table: "DictionaryEntries",
                column: "JlptLevel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Kanjis_Character",
                table: "Kanjis");

            migrationBuilder.DropIndex(
                name: "IX_Kanjis_Level",
                table: "Kanjis");

            migrationBuilder.DropIndex(
                name: "IX_Items_CreatedAt",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_HashKey",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_Japanese_Reading",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_Type",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_DictionaryEntries_CreatedAt",
                table: "DictionaryEntries");

            migrationBuilder.DropIndex(
                name: "IX_DictionaryEntries_JlptLevel",
                table: "DictionaryEntries");
        }
    }
}
