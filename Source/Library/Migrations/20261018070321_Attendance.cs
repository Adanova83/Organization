using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class Attendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attendance",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionSlotId = table.Column<Guid>(type: "uuid", nullable: true),
                    EventId = table.Column<Guid>(type: "uuid", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Method = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendance", x => x.Id);
                    table.CheckConstraint("CK_Attendance_Target", "(\"SessionSlotId\" IS NULL) <> (\"EventId\" IS NULL)");
                    table.ForeignKey(
                        name: "FK_Attendance_ActivitySessionSlot_SessionSlotId",
                        column: x => x.SessionSlotId,
                        principalSchema: "Organization",
                        principalTable: "ActivitySessionSlot",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Attendance_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "Organization",
                        principalTable: "Event",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Attendance_Member_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "Authentication",
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_EventId_MemberId",
                schema: "Organization",
                table: "Attendance",
                columns: new[] { "EventId", "MemberId" },
                unique: true,
                filter: "\"Audit_DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_MemberId",
                schema: "Organization",
                table: "Attendance",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_SessionSlotId_Date_MemberId",
                schema: "Organization",
                table: "Attendance",
                columns: new[] { "SessionSlotId", "Date", "MemberId" },
                unique: true,
                filter: "\"Audit_DeletedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendance",
                schema: "Organization");
        }
    }
}
