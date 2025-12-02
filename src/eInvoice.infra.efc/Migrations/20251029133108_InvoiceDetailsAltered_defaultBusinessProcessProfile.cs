using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eInvoice.infra.efc.Migrations
{
    /// <inheritdoc />
    public partial class InvoiceDetailsAltered_defaultBusinessProcessProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "BusinessProcessProfileId",
                table: "InvoiceDetails",
                type: "int",
                nullable: false,
                defaultValue: 2,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "BusinessProcessProfileId",
                table: "InvoiceDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 2);
        }
    }
}
