using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class SchoolYearZone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Zone",
                schema: "Organization",
                table: "SchoolYear",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "B");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Zone",
                schema: "Organization",
                table: "SchoolYear");
        }
    }
}
