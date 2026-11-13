using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class AttendanceSlotsOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Plus d'appel pour les événements : leurs présences éventuelles (essais) sont supprimées.
            migrationBuilder.Sql("DELETE FROM \"Organization\".\"Attendance\" WHERE \"SessionSlotId\" IS NULL;");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendance_ActivitySessionSlot_SessionSlotId",
                schema: "Organization",
                table: "Attendance");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendance_Event_EventId",
                schema: "Organization",
                table: "Attendance");

            migrationBuilder.DropIndex(
                name: "IX_Attendance_EventId_MemberId",
                schema: "Organization",
                table: "Attendance");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Attendance_Target",
                schema: "Organization",
                table: "Attendance");

            migrationBuilder.DropColumn(
                name: "EventId",
                schema: "Organization",
                table: "Attendance");

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionSlotId",
                schema: "Organization",
                table: "Attendance",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendance_ActivitySessionSlot_SessionSlotId",
                schema: "Organization",
                table: "Attendance",
                column: "SessionSlotId",
                principalSchema: "Organization",
                principalTable: "ActivitySessionSlot",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendance_ActivitySessionSlot_SessionSlotId",
                schema: "Organization",
                table: "Attendance");

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionSlotId",
                schema: "Organization",
                table: "Attendance",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "EventId",
                schema: "Organization",
                table: "Attendance",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_EventId_MemberId",
                schema: "Organization",
                table: "Attendance",
                columns: new[] { "EventId", "MemberId" },
                unique: true,
                filter: "\"Audit_DeletedAt\" IS NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Attendance_Target",
                schema: "Organization",
                table: "Attendance",
                sql: "(\"SessionSlotId\" IS NULL) <> (\"EventId\" IS NULL)");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendance_ActivitySessionSlot_SessionSlotId",
                schema: "Organization",
                table: "Attendance",
                column: "SessionSlotId",
                principalSchema: "Organization",
                principalTable: "ActivitySessionSlot",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendance_Event_EventId",
                schema: "Organization",
                table: "Attendance",
                column: "EventId",
                principalSchema: "Organization",
                principalTable: "Event",
                principalColumn: "Id");
        }
    }
}
