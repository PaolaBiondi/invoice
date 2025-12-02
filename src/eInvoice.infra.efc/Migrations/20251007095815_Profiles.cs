using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace eInvoice.infra.efc.Migrations
{
    /// <inheritdoc />
    public partial class Profiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessProcessProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessProcessProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessProcessProfileId = table.Column<int>(type: "int", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_BusinessProcessProfiles_BusinessProcessProfileId",
                        column: x => x.BusinessProcessProfileId,
                        principalTable: "BusinessProcessProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_Invoices_InvoiceNumber",
                        column: x => x.InvoiceNumber,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceNumber");
                });

            migrationBuilder.InsertData(
                table: "BusinessProcessProfiles",
                columns: new[] { "Id", "Created", "Description", "Name", "Updated" },
                values: new object[,]
                {
                    { 1, new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), "izdavanje računa za isporuke robe i usluga prema narudžbenicama, na temelju ugovora", "P1", new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)) },
                    { 2, new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), "periodično izdavanje računa za isporuke robe i usluga na temelju ugovora", "P2", new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)) },
                    { 3, new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), "izdavanje računa za isporuku prema samostalnoj narudžbenici", "P3", new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)) },
                    { 4, new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), "plaćanje unaprijed (avansno plaćanje)", "P4", new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)) },
                    { 5, new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), "plaćanje na licu mjesta (engl. Sport payment)", "P5", new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)) },
                    { 6, new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), "plaćanje prije isporuke, na temelju narudžbenice", "P6", new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)) },
                    { 7, new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), "izdavanje računa s referencama na otpremnicu", "P7", new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)) },
                    { 8, new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), "izdavanje računa s referencama na otpremnicu i primku", "P8", new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)) },
                    { 9, new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), "odobrenja ili računi s negativnim iznosima, izdani zbog raznih razloga, uključujući i povrat prazne ambalaže", "P9", new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)) },
                    { 10, new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), "izdavanje korektivnog računa (storniranje/ispravak računa)", "P10", new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)) },
                    { 11, new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), "izdavanje djelomičnoga i konačnog računa", "P11", new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)) },
                    { 12, new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), "samoizdavanje računa", "P12", new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)) },
                    { 13, new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), "poslovni proces koji definira Kupac", "P99", new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_BusinessProcessProfileId",
                table: "InvoiceDetails",
                column: "BusinessProcessProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_InvoiceNumber",
                table: "InvoiceDetails",
                column: "InvoiceNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceDetails");

            migrationBuilder.DropTable(
                name: "BusinessProcessProfiles");
        }
    }
}
