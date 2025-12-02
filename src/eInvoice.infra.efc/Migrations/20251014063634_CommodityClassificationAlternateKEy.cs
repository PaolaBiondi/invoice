using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eInvoice.infra.efc.Migrations
{
    /// <inheritdoc />
    public partial class CommodityClassificationAlternateKEy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_CommodityClassifications_Event_Tariff",
                table: "CommodityClassifications",
                columns: new[] { "Event", "Tariff" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_CommodityClassifications_Event_Tariff",
                table: "CommodityClassifications");
        }
    }
}
