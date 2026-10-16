using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class SubscriptionCapacity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                schema: "Organization",
                table: "ProjectSlot",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                schema: "Organization",
                table: "Event",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                schema: "Organization",
                table: "ActivitySessionSlot",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Capacity",
                schema: "Organization",
                table: "ProjectSlot");

            migrationBuilder.DropColumn(
                name: "Capacity",
                schema: "Organization",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "Capacity",
                schema: "Organization",
                table: "ActivitySessionSlot");
        }
    }
}
