using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eInvoice.infra.efc.Migrations
{
    /// Because legacy Billing has InvoiceNumber in parent table set as PK and FK (instead of Id column). The record primary key changes in the some stored procedures. 
    /// in first moment it is timestamp and later this timestamp is replaced by increment number.
    /// Therefore, to keep referential integrity, we need to set ON UPDATE CASCADE on
    /// It is necessary to create ON UPDATE CASCADE manually via SQL since EF Core does not support it directly.
    /// 
    /// <inheritdoc />
    public partial class InvoiceAttachmentsAltered_DefaultValueForCreated_OnUpdateCascade : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Created",
                table: "InvoiceAttachments",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.DropForeignKey(name: "FK_InvoiceAttachments_Invoices_InvoiceNumber",
                                            table: "InvoiceAttachments");

            migrationBuilder.Sql(
                @"ALTER TABLE [dbo].[InvoiceAttachments]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceAttachments_Invoices_InvoiceNumber] FOREIGN KEY([InvoiceNumber])
                  REFERENCES [dbo].[Invoices] ([InvoiceNumber])
                  ON UPDATE CASCADE
                  ON DELETE CASCADE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Created",
                table: "InvoiceAttachments",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.DropForeignKey(name: "FK_InvoiceAttachments_Invoices_InvoiceNumber",
                                            table: "InvoiceAttachments");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceAttachments_Invoices_InvoiceNumber",
                table: "InvoiceAttachments",
                column: "InvoiceNumber",
                principalTable: "Invoices",
                principalColumn: "InvoiceNumber",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
