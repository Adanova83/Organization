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
                name: "Organization");

            migrationBuilder.EnsureSchema(
                name: "Chat");

            migrationBuilder.EnsureSchema(
                name: "Authentication");

            migrationBuilder.CreateTable(
                name: "Chat",
                schema: "Chat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    IsGroup = table.Column<bool>(type: "boolean", nullable: false),
                    Cht_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Cht_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Cht_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chat", x => x.Id);
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
                    Cnt_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Cnt_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Cnt_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
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
                    Hld_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Hld_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Hld_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holiday", x => x.Id);
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
                    Org_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Org_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Org_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Id);
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
                    Spc_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Spc_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Spc_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Space", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Member",
                schema: "Authentication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Picture = table.Column<string>(type: "text", nullable: true),
                    RGBColor = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Birthdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    HashedPassword = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    AskedNewPassword = table.Column<Guid>(type: "uuid", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Addr_Line1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Addr_Line2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Addr_Line3 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Addr_City = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Addr_PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Addr_CountryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Mmb_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Mmb_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Mmb_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "Role",
                schema: "Authentication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Picture = table.Column<string>(type: "text", nullable: true),
                    RGBColor = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Birthdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Addr_Line1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Addr_Line2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Addr_Line3 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Addr_City = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Addr_PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Addr_CountryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rol_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Rol_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Rol_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Role_Country_Addr_CountryId",
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
                    Act_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Act_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Act_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "OrganizationSession",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Ogs_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ogs_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ogs_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Prj_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Prj_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Prj_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "ChatMember",
                schema: "Chat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    ChatId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Cmm_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Cmm_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Cmm_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMember_Chat_ChatId",
                        column: x => x.ChatId,
                        principalSchema: "Chat",
                        principalTable: "Chat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMember_Member_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "Authentication",
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessage",
                schema: "Chat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Body = table.Column<string>(type: "text", nullable: false),
                    SendDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Chm_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Chm_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Chm_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessage_Member_SenderId",
                        column: x => x.SenderId,
                        principalSchema: "Authentication",
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Document",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Doc_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Doc_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Doc_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Document_Member_MemberId",
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
                    Nte_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Nte_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Nte_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Asi_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Asi_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Asi_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                        name: "FK_ActivitySession_OrganizationSession_OrganizationSessionId",
                        column: x => x.OrganizationSessionId,
                        principalSchema: "Organization",
                        principalTable: "OrganizationSession",
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
                    Ctb_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ctb_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ctb_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    OrganizationSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpaceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Evt_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Evt_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Evt_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_OrganizationSession_OrganizationSessionId",
                        column: x => x.OrganizationSessionId,
                        principalSchema: "Organization",
                        principalTable: "OrganizationSession",
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
                    Prs_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Prs_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Prs_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "ChatMessageRecipient",
                schema: "Chat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReadDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ChatMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Cmr_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Cmr_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Cmr_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessageRecipient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessageRecipient_ChatMessage_ChatMessageId",
                        column: x => x.ChatMessageId,
                        principalSchema: "Chat",
                        principalTable: "ChatMessage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessageRecipient_Member_RecipentId",
                        column: x => x.RecipentId,
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
                    Ntl_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ntl_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ntl_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    SpaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Asl_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Asl_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Asl_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    Eva_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Eva_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Eva_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    Evs_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Evs_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Evs_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "ProjectSlotAnimation",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectSlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Psa_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Psa_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Psa_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    Pss_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Pss_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Pss_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    Asa_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Asa_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Asa_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    Ass_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ass_LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ass_DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active_Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "IX_ActivitySession_OrganizationSessionId",
                schema: "Organization",
                table: "ActivitySession",
                column: "OrganizationSessionId");

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
                name: "IX_ChatMember_ChatId",
                schema: "Chat",
                table: "ChatMember",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMember_MemberId",
                schema: "Chat",
                table: "ChatMember",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_SenderId",
                schema: "Chat",
                table: "ChatMessage",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessageRecipient_ChatMessageId",
                schema: "Chat",
                table: "ChatMessageRecipient",
                column: "ChatMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessageRecipient_RecipentId",
                schema: "Chat",
                table: "ChatMessageRecipient",
                column: "RecipentId");

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
                name: "IX_Document_MemberId",
                schema: "Organization",
                table: "Document",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Code",
                schema: "Organization",
                table: "Event",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_OrganizationSessionId",
                schema: "Organization",
                table: "Event",
                column: "OrganizationSessionId");

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
                name: "IX_Role_Addr_CountryId",
                schema: "Authentication",
                table: "Role",
                column: "Addr_CountryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivitySessionSlotAnimation",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "ActivitySessionSlotSubscription",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "ChatMember",
                schema: "Chat");

            migrationBuilder.DropTable(
                name: "ChatMessageRecipient",
                schema: "Chat");

            migrationBuilder.DropTable(
                name: "Contribution",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "Document",
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
                name: "ActivitySessionSlot",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "Chat",
                schema: "Chat");

            migrationBuilder.DropTable(
                name: "ChatMessage",
                schema: "Chat");

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
                name: "OrganizationSession",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "Country",
                schema: "Authentication");

            migrationBuilder.DropTable(
                name: "Organization",
                schema: "Organization");
        }
    }
}
