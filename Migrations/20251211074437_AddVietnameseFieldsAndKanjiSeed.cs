using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JapaneseTrainer.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddVietnameseFieldsAndKanjiSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HanViet",
                table: "Kanjis",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeaningVietnamese",
                table: "Kanjis",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeaningVietnamese",
                table: "Items",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeaningVietnamese",
                table: "DictionaryEntries",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HanViet",
                table: "Kanjis");

            migrationBuilder.DropColumn(
                name: "MeaningVietnamese",
                table: "Kanjis");

            migrationBuilder.DropColumn(
                name: "MeaningVietnamese",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "MeaningVietnamese",
                table: "DictionaryEntries");
        }
    }
}
