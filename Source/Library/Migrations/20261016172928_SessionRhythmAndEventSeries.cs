using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class SessionRhythmAndEventSeries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SeriesId",
                schema: "Organization",
                table: "Event",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DuringSchoolHolidays",
                schema: "Organization",
                table: "ActivitySession",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DuringTermTime",
                schema: "Organization",
                table: "ActivitySession",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_SeriesId",
                schema: "Organization",
                table: "Event",
                column: "SeriesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Event_SeriesId",
                schema: "Organization",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "SeriesId",
                schema: "Organization",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "DuringSchoolHolidays",
                schema: "Organization",
                table: "ActivitySession");

            migrationBuilder.DropColumn(
                name: "DuringTermTime",
                schema: "Organization",
                table: "ActivitySession");
        }
    }
}
