using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class FamilyContacts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contact",
                schema: "Authentication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Firstname = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Lastname = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MemberContact",
                schema: "Authentication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContactId = table.Column<Guid>(type: "uuid", nullable: false),
                    Relation = table.Column<int>(type: "integer", nullable: false),
                    IsEmergencyContact = table.Column<bool>(type: "boolean", nullable: false),
                    CanPickUp = table.Column<bool>(type: "boolean", nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberContact", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberContact_Contact_ContactId",
                        column: x => x.ContactId,
                        principalSchema: "Authentication",
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberContact_Member_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "Authentication",
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberContact_ContactId",
                schema: "Authentication",
                table: "MemberContact",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberContact_MemberId_ContactId",
                schema: "Authentication",
                table: "MemberContact",
                columns: new[] { "MemberId", "ContactId" },
                unique: true,
                filter: "\"Audit_DeletedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberContact",
                schema: "Authentication");

            migrationBuilder.DropTable(
                name: "Contact",
                schema: "Authentication");
        }
    }
}
