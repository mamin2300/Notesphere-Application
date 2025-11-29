using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Notesphere.Services.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
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
                name: "NoteExports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NoteId = table.Column<int>(type: "INTEGER", nullable: false),
                    Format = table.Column<string>(type: "TEXT", nullable: false),
                    ExportedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Destination = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteExports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NoteTags",
                columns: table => new
                {
                    NoteId = table.Column<int>(type: "INTEGER", nullable: false),
                    TagId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteTags", x => new { x.NoteId, x.TagId });
                });

            migrationBuilder.CreateTable(
                name: "NoteTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CssKey = table.Column<string>(type: "TEXT", nullable: false),
                    DefaultContent = table.Column<string>(type: "TEXT", nullable: false),
                    IsSystemTemplate = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NoteVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NoteId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    VersionNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteVersions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuickActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Label = table.Column<string>(type: "TEXT", nullable: false),
                    TargetRoute = table.Column<string>(type: "TEXT", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", nullable: true),
                    IsPrimary = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuickActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecurringEvents",
                columns: table => new
                {
                    RecurrenceId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentEventId = table.Column<int>(type: "INTEGER", nullable: false),
                    Pattern = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Interval = table.Column<int>(type: "INTEGER", nullable: false),
                    DaysOfWeek = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RecurrenceStartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RecurrenceEndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MaxOccurrences = table.Column<int>(type: "INTEGER", nullable: true),
                    ExceptionDates = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringEvents", x => x.RecurrenceId);
                });

            migrationBuilder.CreateTable(
                name: "Reminders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ReminderDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workspaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspaces", x => x.Id);
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
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ColorCode = table.Column<string>(type: "TEXT", maxLength: 7, nullable: false),
                    EventType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsAllDay = table.Column<bool>(type: "INTEGER", nullable: false),
                    RecurrenceId = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_Events_RecurringEvents_RecurrenceId",
                        column: x => x.RecurrenceId,
                        principalTable: "RecurringEvents",
                        principalColumn: "RecurrenceId");
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    IsFavorite = table.Column<bool>(type: "INTEGER", nullable: false),
                    TemplateId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notes_StudentUser_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "StudentUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "Conflicts",
                columns: table => new
                {
                    ConflictId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstEventId = table.Column<int>(type: "INTEGER", nullable: false),
                    SecondEventId = table.Column<int>(type: "INTEGER", nullable: false),
                    ConflictStartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ConflictEndTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OverlapDuration = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    ConflictSeverity = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DetectedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsResolved = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conflicts", x => x.ConflictId);
                    table.ForeignKey(
                        name: "FK_Conflicts_Events_FirstEventId",
                        column: x => x.FirstEventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Conflicts_Events_SecondEventId",
                        column: x => x.SecondEventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotePages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NoteId = table.Column<int>(type: "INTEGER", nullable: false),
                    PageNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageData = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotePages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotePages_Notes_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Notes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Conflicts_FirstEventId",
                table: "Conflicts",
                column: "FirstEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Conflicts_SecondEventId",
                table: "Conflicts",
                column: "SecondEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_RecurrenceId",
                table: "Events",
                column: "RecurrenceId");

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
                name: "IX_NotePages_NoteId",
                table: "NotePages",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_StudentUserId",
                table: "Notes",
                column: "StudentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedNotes_GroupSpaceId",
                table: "SharedNotes",
                column: "GroupSpaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Conflicts");

            migrationBuilder.DropTable(
                name: "GroupMembers");

            migrationBuilder.DropTable(
                name: "NoteComments");

            migrationBuilder.DropTable(
                name: "NoteExports");

            migrationBuilder.DropTable(
                name: "NotePages");

            migrationBuilder.DropTable(
                name: "NoteTags");

            migrationBuilder.DropTable(
                name: "NoteTemplates");

            migrationBuilder.DropTable(
                name: "NoteVersions");

            migrationBuilder.DropTable(
                name: "QuickActions");

            migrationBuilder.DropTable(
                name: "Reminders");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Workspaces");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "SharedNotes");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "RecurringEvents");

            migrationBuilder.DropTable(
                name: "GroupSpaces");

            migrationBuilder.DropTable(
                name: "StudentUser");
        }
    }
}
