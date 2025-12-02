using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eInvoice.infra.efc.Migrations
{
    /// <inheritdoc />
    public partial class AlteredInvoiceDetails_MarkRejected : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRejected",
                table: "InvoiceDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RejectedConfirmationMessage",
                table: "InvoiceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RejectedFiscalizationDateTime",
                table: "InvoiceDetails",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.Sql("""
                USE [ICTSIbilling]
                GO
                /****** Object:  StoredProcedure [dbo].[inv_InsertOrUpdateInvoiceDetail]    Script Date: 17/11/2025 09:39:38 ******/
                SET ANSI_NULLS ON
                GO
                SET QUOTED_IDENTIFIER ON
                GO
                ALTER PROCEDURE [dbo].[inv_InsertOrUpdateInvoiceDetail]
                    @BusinessProcessProfileId INT = NULL,
                    @InvoiceNumber NVARCHAR(30),
                    @SendingStatus INT = NULL,
                	@MarkAsPaid bit = null,	
                    @MarkAsRejected bit = null,
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

                	if (@MarkAsPaid is not null)
                	begin
                	   UPDATE InvoiceDetails SET
                		IsPaid = @MarkAsPaid,
                        Updated = SYSDATETIMEOFFSET()
                	   WHERE InvoiceNumber = @InvoiceNumber

                	   insert into changeHistory ([Description], gkey, [table], [field], oldValue, newValue, [user]) 
                	   values ('update', @gkey, 'InvoiceDetails', getdate(), 'IsPaid', @MarkAsPaid, @operator)
                	End

                	if (@MarkAsRejected is not null)
                	begin
                	   UPDATE InvoiceDetails SET
                		IsRejected = @MarkAsRejected,
                        Updated = SYSDATETIMEOFFSET()
                	   WHERE InvoiceNumber = @InvoiceNumber
                
                	   insert into changeHistory ([Description], gkey, [table], [field], oldValue, newValue, [user]) 
                	   values ('update', @gkey, 'InvoiceDetails', getdate(), 'IsRejected', @MarkAsRejected, @operator)
                	End
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRejected",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "RejectedConfirmationMessage",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "RejectedFiscalizationDateTime",
                table: "InvoiceDetails");

            migrationBuilder.Sql("""
                USE [ICTSIbilling]
                GO
                /****** Object:  StoredProcedure [dbo].[inv_InsertOrUpdateInvoiceDetail]    Script Date: 17/11/2025 09:39:38 ******/
                SET ANSI_NULLS ON
                GO
                SET QUOTED_IDENTIFIER ON
                GO
                ALTER PROCEDURE [dbo].[inv_InsertOrUpdateInvoiceDetail]
                    @BusinessProcessProfileId INT = NULL,
                    @InvoiceNumber NVARCHAR(30),
                    @SendingStatus INT = NULL,
                	@MarkAsPaid bit = null,
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

                	if (@MarkAsPaid is not null)
                	begin
                	   UPDATE InvoiceDetails SET
                		IsPaid = @MarkAsPaid,
                        Updated = SYSDATETIMEOFFSET()
                	   WHERE InvoiceNumber = @InvoiceNumber

                	   insert into changeHistory ([Description], gkey, [table], [field], oldValue, newValue, [user]) 
                	   values ('update', @gkey, 'InvoiceDetails', getdate(), 'IsPaid', @MarkAsPaid, @operator)
                	End
                END
                """);
        }
    }
}
