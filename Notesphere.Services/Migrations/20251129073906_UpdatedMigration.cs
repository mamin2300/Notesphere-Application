using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Notesphere.Services.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isSystemTemplate",
                table: "NoteTemplates",
                newName: "IsSystemTemplate");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "NoteTemplates",
                newName: "DefaultContent");

            migrationBuilder.AddColumn<string>(
                name: "CssKey",
                table: "NoteTemplates",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "NoteTemplates",
                type: "TEXT",
                nullable: true);

            migrationBuilder.InsertData(
                table: "NoteTemplates",
                columns: new[] { "Id", "CssKey", "DefaultContent", "Description", "IsSystemTemplate", "Name" },
                values: new object[,]
                {
                    { 1, "lined", "", "Simple ruled notebook page", true, "Classic lined" },
                    { 2, "dotgrid", "", "For bullet journaling and sketches", true, "Dot grid" },
                    { 3, "cornell", "Topic:\nDate:\n\n[Main notes]\n\nSummary:", "Cue, notes, and summary layout", true, "Cornell notes" },
                    { 4, "blank", "", "Plain, no guides", true, "Minimal blank" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NoteTemplates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "NoteTemplates",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "NoteTemplates",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "NoteTemplates",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "CssKey",
                table: "NoteTemplates");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "NoteTemplates");

            migrationBuilder.RenameColumn(
                name: "IsSystemTemplate",
                table: "NoteTemplates",
                newName: "isSystemTemplate");

            migrationBuilder.RenameColumn(
                name: "DefaultContent",
                table: "NoteTemplates",
                newName: "Content");
        }
    }
}
