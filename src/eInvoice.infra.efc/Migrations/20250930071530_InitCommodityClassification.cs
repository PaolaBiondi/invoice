using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eInvoice.infra.efc.Migrations
{
    /// <inheritdoc />
    public partial class InitCommodityClassification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommodityClassifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassificationCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tariff = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    Event = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommodityClassifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommodityClassifications_tariffs_Tariff_Event",
                        columns: x => new { x.Tariff, x.Event },
                        principalTable: "tariffs",
                        principalColumns: new[] { "Tariff", "Event" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommodityClassifications_Tariff_Event",
                table: "CommodityClassifications",
                columns: new[] { "Tariff", "Event" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommodityClassifications");
        }
    }
}
