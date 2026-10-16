using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class PartialUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SchoolYear_Code",
                schema: "Organization",
                table: "SchoolYear");

            migrationBuilder.DropIndex(
                name: "IX_Role_Code",
                schema: "Authentication",
                table: "Role");

            migrationBuilder.DropIndex(
                name: "IX_Project_Code",
                schema: "Organization",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Member_Email",
                schema: "Authentication",
                table: "Member");

            migrationBuilder.DropIndex(
                name: "IX_Member_ExternalId",
                schema: "Authentication",
                table: "Member");

            migrationBuilder.DropIndex(
                name: "IX_Event_Code",
                schema: "Organization",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Country_CodeAlpha2",
                schema: "Authentication",
                table: "Country");

            migrationBuilder.DropIndex(
                name: "IX_Country_CodeAlpha3",
                schema: "Authentication",
                table: "Country");

            migrationBuilder.DropIndex(
                name: "IX_Badge_Identifier",
                schema: "Access",
                table: "Badge");

            migrationBuilder.DropIndex(
                name: "IX_Activity_Code",
                schema: "Organization",
                table: "Activity");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolYear_Code",
                schema: "Organization",
                table: "SchoolYear",
                column: "Code",
                unique: true,
                filter: "\"Audit_DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Code",
                schema: "Authentication",
                table: "Role",
                column: "Code",
                unique: true,
                filter: "\"Audit_DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Code",
                schema: "Organization",
                table: "Project",
                column: "Code",
                unique: true,
                filter: "\"Audit_DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Member_Email",
                schema: "Authentication",
                table: "Member",
                column: "Email",
                unique: true,
                filter: "\"Audit_DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Member_ExternalId",
                schema: "Authentication",
                table: "Member",
                column: "ExternalId",
                unique: true,
                filter: "\"Audit_DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Code",
                schema: "Organization",
                table: "Event",
                column: "Code",
                unique: true,
                filter: "\"Audit_DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Country_CodeAlpha2",
                schema: "Authentication",
                table: "Country",
                column: "CodeAlpha2",
                unique: true,
                filter: "\"Audit_DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Country_CodeAlpha3",
                schema: "Authentication",
                table: "Country",
                column: "CodeAlpha3",
                unique: true,
                filter: "\"Audit_DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Badge_Identifier",
                schema: "Access",
                table: "Badge",
                column: "Identifier",
                unique: true,
                filter: "\"Audit_DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_Code",
                schema: "Organization",
                table: "Activity",
                column: "Code",
                unique: true,
                filter: "\"Audit_DeletedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SchoolYear_Code",
                schema: "Organization",
                table: "SchoolYear");

            migrationBuilder.DropIndex(
                name: "IX_Role_Code",
                schema: "Authentication",
                table: "Role");

            migrationBuilder.DropIndex(
                name: "IX_Project_Code",
                schema: "Organization",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Member_Email",
                schema: "Authentication",
                table: "Member");

            migrationBuilder.DropIndex(
                name: "IX_Member_ExternalId",
                schema: "Authentication",
                table: "Member");

            migrationBuilder.DropIndex(
                name: "IX_Event_Code",
                schema: "Organization",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Country_CodeAlpha2",
                schema: "Authentication",
                table: "Country");

            migrationBuilder.DropIndex(
                name: "IX_Country_CodeAlpha3",
                schema: "Authentication",
                table: "Country");

            migrationBuilder.DropIndex(
                name: "IX_Badge_Identifier",
                schema: "Access",
                table: "Badge");

            migrationBuilder.DropIndex(
                name: "IX_Activity_Code",
                schema: "Organization",
                table: "Activity");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolYear_Code",
                schema: "Organization",
                table: "SchoolYear",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Role_Code",
                schema: "Authentication",
                table: "Role",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Project_Code",
                schema: "Organization",
                table: "Project",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Member_Email",
                schema: "Authentication",
                table: "Member",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Member_ExternalId",
                schema: "Authentication",
                table: "Member",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_Code",
                schema: "Organization",
                table: "Event",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Country_CodeAlpha2",
                schema: "Authentication",
                table: "Country",
                column: "CodeAlpha2",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Country_CodeAlpha3",
                schema: "Authentication",
                table: "Country",
                column: "CodeAlpha3",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Badge_Identifier",
                schema: "Access",
                table: "Badge",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activity_Code",
                schema: "Organization",
                table: "Activity",
                column: "Code",
                unique: true);
        }
    }
}
