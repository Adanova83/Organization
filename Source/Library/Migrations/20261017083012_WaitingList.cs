using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class WaitingList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWaiting",
                schema: "Organization",
                table: "ProjectSlotSubscription",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWaiting",
                schema: "Organization",
                table: "EventSubscription",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWaiting",
                schema: "Organization",
                table: "ActivitySessionSlotSubscription",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWaiting",
                schema: "Organization",
                table: "ProjectSlotSubscription");

            migrationBuilder.DropColumn(
                name: "IsWaiting",
                schema: "Organization",
                table: "EventSubscription");

            migrationBuilder.DropColumn(
                name: "IsWaiting",
                schema: "Organization",
                table: "ActivitySessionSlotSubscription");
        }
    }
}
