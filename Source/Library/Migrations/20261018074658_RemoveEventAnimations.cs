using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEventAnimations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventAnimation",
                schema: "Organization");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventAnimation",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAnimation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventAnimation_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "Organization",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventAnimation_Member_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "Authentication",
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventAnimation_EventId",
                schema: "Organization",
                table: "EventAnimation",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAnimation_MemberId",
                schema: "Organization",
                table: "EventAnimation",
                column: "MemberId");
        }
    }
}
