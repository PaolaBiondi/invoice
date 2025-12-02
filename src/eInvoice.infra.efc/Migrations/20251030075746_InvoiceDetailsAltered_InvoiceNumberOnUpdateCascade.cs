using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eInvoice.infra.efc.Migrations
{
    /// <inheritdoc />
    public partial class InvoiceDetailsAltered_InvoiceNumberOnUpdateCascade : Migration
    {
        /// Because legacy Billing has InvoiceNumber in parent table set as PK and FK (instead of Id column). The record primary key changes in the stored procedures. 
        /// in first moment it is timestamp and later this timestamp is replaced by increment number.
        /// Therefore, to keep referential integrity, we need to set ON UPDATE CASCADE on
        /// It is necessary to create ON UPDATE CASCADE manually via SQL since EF Core does not support it directly.
        /// 
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_Invoices_InvoiceNumber",
                table: "InvoiceDetails");

            migrationBuilder.Sql(
                @"ALTER TABLE [dbo].[InvoiceDetails]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceDetails_Invoices_InvoiceNumber] FOREIGN KEY([InvoiceNumber])
                  REFERENCES [dbo].[Invoices] ([InvoiceNumber])
                  ON UPDATE CASCADE
                  ON DELETE CASCADE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_Invoices_InvoiceNumber",
                table: "InvoiceDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_Invoices_InvoiceNumber",
                table: "InvoiceDetails",
                column: "InvoiceNumber",
                principalTable: "Invoices",
                principalColumn: "InvoiceNumber",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
