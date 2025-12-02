using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eInvoice.infra.efc.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInvoiceAttachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "InvoiceAttachments",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "InvoiceAttachments");
        }
    }
}
