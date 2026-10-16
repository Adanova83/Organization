using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class ProjectRhythm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DuringSchoolHolidays",
                schema: "Organization",
                table: "Project",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DuringTermTime",
                schema: "Organization",
                table: "Project",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuringSchoolHolidays",
                schema: "Organization",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "DuringTermTime",
                schema: "Organization",
                table: "Project");
        }
    }
}
