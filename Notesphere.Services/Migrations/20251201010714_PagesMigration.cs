using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notesphere.Services.Migrations
{
    /// <inheritdoc />
    public partial class PagesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TextBoxesJson",
                table: "NotePages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextBoxesJson",
                table: "NotePages");
        }
    }
}
