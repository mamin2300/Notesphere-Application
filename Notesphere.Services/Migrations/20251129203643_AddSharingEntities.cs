using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notesphere.Services.Migrations
{
    /// <inheritdoc />
    public partial class AddSharingEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupSpaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupSpaces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GroupSpaceId = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupMembers_GroupSpaces_GroupSpaceId",
                        column: x => x.GroupSpaceId,
                        principalTable: "GroupSpaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SharedNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NoteId = table.Column<int>(type: "INTEGER", nullable: false),
                    SharedByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    SharedWithUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    GroupSpaceId = table.Column<int>(type: "INTEGER", nullable: true),
                    TargetType = table.Column<int>(type: "INTEGER", nullable: false),
                    CanEdit = table.Column<bool>(type: "INTEGER", nullable: false),
                    SharedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SharedNotes_GroupSpaces_GroupSpaceId",
                        column: x => x.GroupSpaceId,
                        principalTable: "GroupSpaces",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NoteComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NoteId = table.Column<int>(type: "INTEGER", nullable: false),
                    AuthorUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ParentCommentId = table.Column<int>(type: "INTEGER", nullable: true),
                    SharedNoteId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NoteComments_NoteComments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "NoteComments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NoteComments_SharedNotes_SharedNoteId",
                        column: x => x.SharedNoteId,
                        principalTable: "SharedNotes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_GroupSpaceId",
                table: "GroupMembers",
                column: "GroupSpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_NoteComments_ParentCommentId",
                table: "NoteComments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_NoteComments_SharedNoteId",
                table: "NoteComments",
                column: "SharedNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedNotes_GroupSpaceId",
                table: "SharedNotes",
                column: "GroupSpaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupMembers");

            migrationBuilder.DropTable(
                name: "NoteComments");

            migrationBuilder.DropTable(
                name: "SharedNotes");

            migrationBuilder.DropTable(
                name: "GroupSpaces");
        }
    }
}
