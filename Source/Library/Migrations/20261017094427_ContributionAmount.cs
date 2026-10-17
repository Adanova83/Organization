using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class ContributionAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Amount",
                schema: "Organization",
                table: "Contribution",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            // Avant ce champ, une cotisation enregistrée valait paiement complet : montant = tarif de la saison.
            migrationBuilder.Sql(
                "UPDATE \"Organization\".\"Contribution\" c SET \"Amount\" = s.\"Price\" " +
                "FROM \"Organization\".\"OrganizationSession\" s WHERE s.\"Id\" = c.\"OrganizationSessionId\";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                schema: "Organization",
                table: "Contribution");
        }
    }
}
