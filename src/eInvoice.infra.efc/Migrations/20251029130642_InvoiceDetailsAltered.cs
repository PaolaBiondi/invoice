using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eInvoice.infra.efc.Migrations
{
    /// <inheritdoc />
    public partial class InvoiceDetailsAltered : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SendingStatus",
                table: "InvoiceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.RenameColumn(
                name: "Updateded",
                table: "InvoiceAttachments",
                newName: "Updated");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Created",
                table: "InvoiceDetails",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Updated",
                table: "InvoiceDetails",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.Sql("""
                CREATE PROCEDURE inv_InsertOrUpdateInvoiceDetail
                    @BusinessProcessProfileId INT = NULL,
                    @InvoiceNumber NVARCHAR(30),
                    @SendingStatus INT = NULL,
                	@operator NVARCHAR(30) = NULL
                AS
                BEGIN
                    SET NOCOUNT ON;
                	Declare @oldBusinessProcessProfileId as int = null,
                			@oldSendingStatus as int = null,
                			@gkey as int = null

                	-- ensure record exists
                	if not exists (select 1 from InvoiceDetails where InvoiceNumber = @InvoiceNumber)
                	begin
                		INSERT InvoiceDetails (InvoiceNumber, Updated)
                        VALUES (@InvoiceNumber, SYSDATETIMEOFFSET());
                	end
                	else 
                	begin 
                		select @oldBusinessProcessProfileId = BusinessProcessProfileId,
                				@oldSendingStatus = SendingStatus
                		 from InvoiceDetails
                		 where InvoiceNumber = @InvoiceNumber
                	end

                	-- changeHistory relies on invoice.Id not InvoiceNumber
                	select @gkey = ID from Invoices where InvoiceNumber = @InvoiceNumber

                	if (@BusinessProcessProfileId is not null)
                	begin
                		UPDATE InvoiceDetails SET 
                			BusinessProcessProfileId = @BusinessProcessProfileId,
                            Updated = SYSDATETIMEOFFSET()
                		WHERE InvoiceNumber = @InvoiceNumber

                		 insert into changeHistory ([Description], gkey, [table], [field], oldValue, newValue, [user]) 
                		 values ('update', @gkey, 'InvoiceDetails', getdate(), 'BusinessProcessProfileId', @BusinessProcessProfileId, @operator)
                	end

                	if (@SendingStatus is not null)
                	begin
                	   UPDATE InvoiceDetails SET
                		SendingStatus = @SendingStatus,
                        Updated = SYSDATETIMEOFFSET()
                	   WHERE InvoiceNumber = @InvoiceNumber

                	   insert into changeHistory ([Description], gkey, [table], [field], oldValue, newValue, [user]) 
                	   values ('update', @gkey, 'InvoiceDetails', getdate(), 'SendingStatus', @SendingStatus, @operator)
                	End
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS inv_InsertOrUpdateInvoiceDetail");

            migrationBuilder.DropColumn(
                name: "SendingStatus",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "InvoiceDetails");

            migrationBuilder.RenameColumn(
                name: "Updated",
                table: "InvoiceAttachments",
                newName: "Updateded");
        }
    }
}
