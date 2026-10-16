using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Access");

            migrationBuilder.EnsureSchema(
                name: "Organization");

            migrationBuilder.EnsureSchema(
                name: "Authentication");

            migrationBuilder.CreateTable(
                name: "Application",
                schema: "Access",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ApiKey = table.Column<string>(type: "text", nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Badge",
                schema: "Access",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Identifier = table.Column<string>(type: "text", nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Badge", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Country",
                schema: "Authentication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodeAlpha2 = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    CodeAlpha3 = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Picture = table.Column<string>(type: "text", nullable: true),
                    RGBColor = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "Authentication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Picture = table.Column<string>(type: "text", nullable: true),
                    RGBColor = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolYear",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolYear", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Space",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Picture = table.Column<string>(type: "text", nullable: true),
                    RGBColor = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Space", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccessEvent",
                schema: "Access",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BadgeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExitedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsAuthorized = table.Column<bool>(type: "boolean", nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessEvent_Application_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "Access",
                        principalTable: "Application",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessEvent_Badge_BadgeId",
                        column: x => x.BadgeId,
                        principalSchema: "Access",
                        principalTable: "Badge",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Member",
                schema: "Authentication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Firstname = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Lastname = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Picture = table.Column<string>(type: "text", nullable: true),
                    RGBColor = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Birthdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Addr_Line1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Addr_Line2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Addr_Line3 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Addr_City = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Addr_PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Addr_CountryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Member_Country_Addr_CountryId",
                        column: x => x.Addr_CountryId,
                        principalSchema: "Authentication",
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Activity",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Picture = table.Column<string>(type: "text", nullable: true),
                    RGBColor = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    AgeMinimum = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activity_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Organization",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Holiday",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SchoolYearId = table.Column<Guid>(type: "uuid", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holiday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Holiday_SchoolYear_SchoolYearId",
                        column: x => x.SchoolYearId,
                        principalSchema: "Organization",
                        principalTable: "SchoolYear",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrganizationSession",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationSession_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Organization",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationSession_SchoolYear_SchoolYearId",
                        column: x => x.SchoolYearId,
                        principalSchema: "Organization",
                        principalTable: "SchoolYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Picture = table.Column<string>(type: "text", nullable: true),
                    RGBColor = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    AgeMinimum = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SchoolYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Project_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Organization",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Project_SchoolYear_SchoolYearId",
                        column: x => x.SchoolYearId,
                        principalSchema: "Organization",
                        principalTable: "SchoolYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Picture = table.Column<string>(type: "text", nullable: true),
                    RGBColor = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AgeMinimum = table.Column<int>(type: "integer", nullable: true),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpaceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Organization",
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Event_SchoolYear_SchoolYearId",
                        column: x => x.SchoolYearId,
                        principalSchema: "Organization",
                        principalTable: "SchoolYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Event_Space_SpaceId",
                        column: x => x.SpaceId,
                        principalSchema: "Organization",
                        principalTable: "Space",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BadgeAssignation",
                schema: "Access",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BadgeId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BadgeAssignation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BadgeAssignation_Badge_BadgeId",
                        column: x => x.BadgeId,
                        principalSchema: "Access",
                        principalTable: "Badge",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BadgeAssignation_Member_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "Authentication",
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Note",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    RGBBackgroundColor = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Note", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Note_Member_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "Authentication",
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivitySession",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Price = table.Column<float>(type: "real", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivitySession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivitySession_Activity_ActivityId",
                        column: x => x.ActivityId,
                        principalSchema: "Organization",
                        principalTable: "Activity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivitySession_SchoolYear_SchoolYearId",
                        column: x => x.SchoolYearId,
                        principalSchema: "Organization",
                        principalTable: "SchoolYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contribution",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    PayedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contribution", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contribution_Member_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "Authentication",
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contribution_OrganizationSession_OrganizationSessionId",
                        column: x => x.OrganizationSessionId,
                        principalSchema: "Organization",
                        principalTable: "OrganizationSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectSlot",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpaceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSlot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectSlot_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectSlot_Space_SpaceId",
                        column: x => x.SpaceId,
                        principalSchema: "Organization",
                        principalTable: "Space",
                        principalColumn: "Id");
                });

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
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "EventSubscription",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSubscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventSubscription_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "Organization",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventSubscription_Member_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "Authentication",
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NoteLink",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NoteId = table.Column<Guid>(type: "uuid", nullable: false),
                    LinkedType = table.Column<string>(type: "text", nullable: false),
                    LinkedId = table.Column<Guid>(type: "uuid", nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NoteLink_Note_NoteId",
                        column: x => x.NoteId,
                        principalSchema: "Organization",
                        principalTable: "Note",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivitySessionSlot",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    Frequency = table.Column<int>(type: "integer", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpaceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivitySessionSlot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivitySessionSlot_ActivitySession_SessionId",
                        column: x => x.SessionId,
                        principalSchema: "Organization",
                        principalTable: "ActivitySession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivitySessionSlot_Space_SpaceId",
                        column: x => x.SpaceId,
                        principalSchema: "Organization",
                        principalTable: "Space",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProjectSlotAnimation",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectSlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSlotAnimation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectSlotAnimation_Member_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "Authentication",
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectSlotAnimation_ProjectSlot_ProjectSlotId",
                        column: x => x.ProjectSlotId,
                        principalSchema: "Organization",
                        principalTable: "ProjectSlot",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectSlotSubscription",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectSlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSlotSubscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectSlotSubscription_Member_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "Authentication",
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectSlotSubscription_ProjectSlot_ProjectSlotId",
                        column: x => x.ProjectSlotId,
                        principalSchema: "Organization",
                        principalTable: "ProjectSlot",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivitySessionSlotAnimation",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionSlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivitySessionSlotAnimation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivitySessionSlotAnimation_ActivitySessionSlot_SessionSlo~",
                        column: x => x.SessionSlotId,
                        principalSchema: "Organization",
                        principalTable: "ActivitySessionSlot",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivitySessionSlotAnimation_Member_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "Authentication",
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivitySessionSlotSubscription",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionSlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Audit_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Audit_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivitySessionSlotSubscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivitySessionSlotSubscription_ActivitySessionSlot_Session~",
                        column: x => x.SessionSlotId,
                        principalSchema: "Organization",
                        principalTable: "ActivitySessionSlot",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivitySessionSlotSubscription_Member_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "Authentication",
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessEvent_ApplicationId",
                schema: "Access",
                table: "AccessEvent",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessEvent_BadgeId",
                schema: "Access",
                table: "AccessEvent",
                column: "BadgeId");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_Code",
                schema: "Organization",
                table: "Activity",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activity_OrganizationId",
                schema: "Organization",
                table: "Activity",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivitySession_ActivityId",
                schema: "Organization",
                table: "ActivitySession",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivitySession_SchoolYearId",
                schema: "Organization",
                table: "ActivitySession",
                column: "SchoolYearId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivitySessionSlot_SessionId",
                schema: "Organization",
                table: "ActivitySessionSlot",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivitySessionSlot_SpaceId",
                schema: "Organization",
                table: "ActivitySessionSlot",
                column: "SpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivitySessionSlotAnimation_MemberId",
                schema: "Organization",
                table: "ActivitySessionSlotAnimation",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivitySessionSlotAnimation_SessionSlotId",
                schema: "Organization",
                table: "ActivitySessionSlotAnimation",
                column: "SessionSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivitySessionSlotSubscription_MemberId",
                schema: "Organization",
                table: "ActivitySessionSlotSubscription",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivitySessionSlotSubscription_SessionSlotId",
                schema: "Organization",
                table: "ActivitySessionSlotSubscription",
                column: "SessionSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_Badge_Identifier",
                schema: "Access",
                table: "Badge",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BadgeAssignation_BadgeId",
                schema: "Access",
                table: "BadgeAssignation",
                column: "BadgeId");

            migrationBuilder.CreateIndex(
                name: "IX_BadgeAssignation_MemberId",
                schema: "Access",
                table: "BadgeAssignation",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Contribution_MemberId",
                schema: "Organization",
                table: "Contribution",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Contribution_OrganizationSessionId",
                schema: "Organization",
                table: "Contribution",
                column: "OrganizationSessionId");

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
                name: "IX_Event_Code",
                schema: "Organization",
                table: "Event",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_OrganizationId",
                schema: "Organization",
                table: "Event",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_SchoolYearId",
                schema: "Organization",
                table: "Event",
                column: "SchoolYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_SpaceId",
                schema: "Organization",
                table: "Event",
                column: "SpaceId");

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

            migrationBuilder.CreateIndex(
                name: "IX_EventSubscription_EventId",
                schema: "Organization",
                table: "EventSubscription",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSubscription_MemberId",
                schema: "Organization",
                table: "EventSubscription",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Holiday_SchoolYearId",
                schema: "Organization",
                table: "Holiday",
                column: "SchoolYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_Addr_CountryId",
                schema: "Authentication",
                table: "Member",
                column: "Addr_CountryId");

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
                name: "IX_Note_MemberId",
                schema: "Organization",
                table: "Note",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_NoteLink_NoteId",
                schema: "Organization",
                table: "NoteLink",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSession_OrganizationId",
                schema: "Organization",
                table: "OrganizationSession",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSession_SchoolYearId",
                schema: "Organization",
                table: "OrganizationSession",
                column: "SchoolYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Code",
                schema: "Organization",
                table: "Project",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Project_OrganizationId",
                schema: "Organization",
                table: "Project",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_SchoolYearId",
                schema: "Organization",
                table: "Project",
                column: "SchoolYearId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSlot_ProjectId",
                schema: "Organization",
                table: "ProjectSlot",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSlot_SpaceId",
                schema: "Organization",
                table: "ProjectSlot",
                column: "SpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSlotAnimation_MemberId",
                schema: "Organization",
                table: "ProjectSlotAnimation",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSlotAnimation_ProjectSlotId",
                schema: "Organization",
                table: "ProjectSlotAnimation",
                column: "ProjectSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSlotSubscription_MemberId",
                schema: "Organization",
                table: "ProjectSlotSubscription",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSlotSubscription_ProjectSlotId",
                schema: "Organization",
                table: "ProjectSlotSubscription",
                column: "ProjectSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Code",
                schema: "Authentication",
                table: "Role",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchoolYear_Code",
                schema: "Organization",
                table: "SchoolYear",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessEvent",
                schema: "Access");

            migrationBuilder.DropTable(
                name: "ActivitySessionSlotAnimation",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "ActivitySessionSlotSubscription",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "BadgeAssignation",
                schema: "Access");

            migrationBuilder.DropTable(
                name: "Contribution",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "EventAnimation",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "EventSubscription",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "Holiday",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "NoteLink",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "ProjectSlotAnimation",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "ProjectSlotSubscription",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "Authentication");

            migrationBuilder.DropTable(
                name: "Application",
                schema: "Access");

            migrationBuilder.DropTable(
                name: "ActivitySessionSlot",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "Badge",
                schema: "Access");

            migrationBuilder.DropTable(
                name: "OrganizationSession",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "Event",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "Note",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "ProjectSlot",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "ActivitySession",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "Member",
                schema: "Authentication");

            migrationBuilder.DropTable(
                name: "Project",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "Space",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "Activity",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "Country",
                schema: "Authentication");

            migrationBuilder.DropTable(
                name: "SchoolYear",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "Organization",
                schema: "Organization");
        }
    }
}
