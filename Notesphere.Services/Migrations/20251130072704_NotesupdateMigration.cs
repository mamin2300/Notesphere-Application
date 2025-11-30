using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notesphere.Services.Migrations
{
    /// <inheritdoc />
    public partial class NotesupdateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "StudentUser",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "StudentUser",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "NoteTemplates",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Lined");

            migrationBuilder.UpdateData(
                table: "NoteTemplates",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CssKey", "Description", "Name" },
                values: new object[] { "dotted", "Dot grid for bullet journaling", "Dotted" });

            migrationBuilder.UpdateData(
                table: "NoteTemplates",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CssKey", "DefaultContent", "Description", "Name" },
                values: new object[] { "boxed", "", "Box/grid style math page", "Grid" });

            migrationBuilder.UpdateData(
                table: "NoteTemplates",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Plain page without guides", "Blank" });

            migrationBuilder.CreateIndex(
                name: "IX_NoteVersions_NoteId",
                table: "NoteVersions",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_NoteTags_TagId",
                table: "NoteTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_TemplateId",
                table: "Notes",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_NoteExports_NoteId",
                table: "NoteExports",
                column: "NoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_NoteExports_Notes_NoteId",
                table: "NoteExports",
                column: "NoteId",
                principalTable: "Notes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_NoteTemplates_TemplateId",
                table: "Notes",
                column: "TemplateId",
                principalTable: "NoteTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_NoteTags_Notes_NoteId",
                table: "NoteTags",
                column: "NoteId",
                principalTable: "Notes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NoteTags_Tags_TagId",
                table: "NoteTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NoteVersions_Notes_NoteId",
                table: "NoteVersions",
                column: "NoteId",
                principalTable: "Notes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NoteExports_Notes_NoteId",
                table: "NoteExports");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_NoteTemplates_TemplateId",
                table: "Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_NoteTags_Notes_NoteId",
                table: "NoteTags");

            migrationBuilder.DropForeignKey(
                name: "FK_NoteTags_Tags_TagId",
                table: "NoteTags");

            migrationBuilder.DropForeignKey(
                name: "FK_NoteVersions_Notes_NoteId",
                table: "NoteVersions");

            migrationBuilder.DropIndex(
                name: "IX_NoteVersions_NoteId",
                table: "NoteVersions");

            migrationBuilder.DropIndex(
                name: "IX_NoteTags_TagId",
                table: "NoteTags");

            migrationBuilder.DropIndex(
                name: "IX_Notes_TemplateId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_NoteExports_NoteId",
                table: "NoteExports");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "StudentUser");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "StudentUser");

            migrationBuilder.UpdateData(
                table: "NoteTemplates",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Classic lined");

            migrationBuilder.UpdateData(
                table: "NoteTemplates",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CssKey", "Description", "Name" },
                values: new object[] { "dotgrid", "For bullet journaling and sketches", "Dot grid" });

            migrationBuilder.UpdateData(
                table: "NoteTemplates",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CssKey", "DefaultContent", "Description", "Name" },
                values: new object[] { "cornell", "Topic:\nDate:\n\n[Main notes]\n\nSummary:", "Cue, notes, and summary layout", "Cornell notes" });

            migrationBuilder.UpdateData(
                table: "NoteTemplates",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Plain, no guides", "Minimal blank" });
        }
    }
}
