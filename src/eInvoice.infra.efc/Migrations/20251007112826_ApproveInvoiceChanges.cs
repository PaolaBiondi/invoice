using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eInvoice.infra.efc.Migrations
{
    /// <inheritdoc />
    public partial class ApproveInvoiceChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
            @"
			USE [ICTSIbilling]
			GO
			/****** Object:  StoredProcedure [dbo].[inv_ApprovingInvoice]    Script Date: 07/10/2025 10:05:21 ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			-- =============================================
			-- Author:		Josip Kubasa
			-- Create date: 07.08.2014
			-- Description:
			-- =============================================
			ALTER PROCEDURE [dbo].[inv_ApprovingInvoice]
				@InvoiceNbr AS NVARCHAR(30),
				@user AS NVARCHAR(20)
			AS
			BEGIN
				DECLARE @BillingCancel NVARCHAR(10),
					@BillingCancelId as int,
					@InvoiceFinalNumber NVARCHAR(25),
					@FiscalizationNumber NVARCHAR(25),
					@invoiceType as int,
					@invoiceID AS int,
					@table as varchar(20),
					@invoiced as smallDateTime	   

				-- Inicijalni podaci
				SELECT @invoiceID = ID, @invoiceType = billing_method, @BillingCancel = [CancelledBillingNumber], @invoiced = Invoiced FROM [ICTSIbilling].[dbo].[Invoices] where InvoiceNumber = @InvoiceNbr
				SET @table = ''

				-- Novi pravi broj računa
				SELECT @InvoiceFinalNumber = dbo.NewInvoiceNumber()

				-- Fiskalni broj računa
				SELECT @FiscalizationNumber = dbo.NewFiscalization()


				BEGIN TRANSACTION
				BEGIN TRY

					-- KreirajTMP tabele
					CREATE TABLE #SAPinvoiceId (id int,altInvoiceNbr nvarchar(30))
					CREATE TABLE #itemsId (id int,SAPid int)
					CREATE TABLE #invoiceElemets (id int)
					CREATE TABLE #elemetsSource (id int,[status] nvarchar(20),[table] char(1))

					-- *********** Zapamti izvorne podatke
					-- Zapamti sve zapise za račun
					INSERT #SAPinvoiceId SELECT ID,@InvoiceFinalNumber+DistrChanel+Division FROM SAPInvoices WHERE InvoiceNumber = @InvoiceNbr
					INSERT #itemsId select i.ID,s.ID FROM InvoiceItems i,SAPInvoices s where s.InvoiceNumber = @InvoiceNbr and s.AltInvoiceNumber = i.AltInvoiceNumber

					-- setiraj brojeve računa na NULL
					-- InvoiceItems
					UPDATE InvoiceItems SET AltInvoiceNumber = NULL where ID in (select id from #itemsId)
					-- SAPinvoice
					UPDATE SAPInvoices SET InvoiceNumber = null,
					AltInvoiceNumber = CAST(@InvoiceFinalNumber AS NVARCHAR(30))+DistrChanel+Division,
					Changed=GETDATE()
					WHERE ID in  (SELECT ID FROM #SAPinvoiceId)

					-- ******** Upiši novi broj računa
					-- Invoice
					UPDATE Invoices SET InvoiceNumber = @InvoiceFinalNumber, Fiscalization = @FiscalizationNumber,InvoiceStatus = 'BILLED',Approved = GETDATE(),ApprovedUser = @user WHERE InvoiceNumber = @InvoiceNbr
					UPDATE Invoices SET CustomerRefNbr = @InvoiceFinalNumber where CustomerRefNbr = @InvoiceNbr
					-- InvoiceAdded
					UPDATE InvoiceAdded SET InvoiceNumber = @InvoiceFinalNumber WHERE InvoiceNumber = @InvoiceNbr
					-- SAPinvoices
					UPDATE SAPInvoices SET InvoiceNumber = @InvoiceFinalNumber,ExtractStatus = '1',Changed=GETDATE() WHERE ID IN (SELECT ID FROM #SAPinvoiceId)

					-- InvoiceItems
					UPDATE InvoiceItems set AltInvoiceNumber = s.altInvoiceNbr FROM #itemsId i, #SAPinvoiceId s where i.id = InvoiceItems.ID  AND s.id = i.SAPid

					-- Ako je račun na kojemu su spajani ostali računi mora se u izvornom računu promjeniti broj računa u polju 'CancelledBillingInvoice'
					UPDATE Invoices SET CancelledBillingNumber = @InvoiceFinalNumber WHERE InvoiceStatus = 'JOINTED' and CancelledBillingNumber = @InvoiceNbr

					-- ********* Potrebno ažurirati sve ostale izvorne tabele
					-- Ovisno o @invoiceType definira se slijed računa za promjenu
					IF (@invoiceType = 1 or @invoiceType = 2 or @invoiceType = 8 or @invoiceType = 9)	-- Brodske operacije/Ostale operacije/energija
					BEGIN
						-- Table BillingEvents/BillingSurcharge
						INSERT #elemetsSource SELECT gkey,[status],CASE WHEN @invoiceType = 9 THEN 'G' ELSE 'E' END FROM BillingEvents s WHERE s.invoice = @InvoiceNbr
						INSERT #elemetsSource SELECT gkey,[status],'V' FROM BillingVessel s WHERE s.invoice = @InvoiceNbr
						UPDATE BillingEvents SET [status] = 'BILLED',invoice = @InvoiceFinalNumber where invoice = @InvoiceNbr
						UPDATE BillingSurcharge SET [status] = 'BILLED' WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table] = 'E')
						UPDATE BillingVessel SET  [status] = 'BILLED',invoice = @InvoiceFinalNumber WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table] = 'V')

						-- Table InvoiceEvents
						UPDATE InvoiceEvents SET [status] = 'BILLED',invoice = @InvoiceFinalNumber FROM #elemetsSource s WHERE s.id = gkey  and invoice = @InvoiceNbr
						SET @table = 'InvoiceEvents'

						-- Sparcsn4
						UPDATE sparcsn4.dbo.argo_chargeable_unit_events SET [status] = 'BILLED',flex_string02 = @InvoiceFinalNumber, flex_date02 = @invoiced
							WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table] = 'E')
						UPDATE sparcsn4.dbo.argo_chargeable_marine_events SET [status] = 'BILLED',flex_string02 = @InvoiceFinalNumber, flex_date02 = @invoiced
							WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table] = 'V')

					END
					IF (@invoiceType = 3)
					BEGIN
						-- Table BillingEvents/BillingSurcharge
						INSERT #elemetsSource SELECT gkey,[status],'S' FROM BillingEvents s WHERE s.invoice = @InvoiceNbr
						UPDATE BillingEvents SET [status] = 'BILLED',invoice = @InvoiceFinalNumber where invoice = @InvoiceNbr
						UPDATE BillingSurcharge SET [status] = 'BILLED' WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource)

						-- Table InvoiceStorage
						UPDATE InvoiceStorage SET [status] = 'BILLED',invoice = @InvoiceFinalNumber FROM #elemetsSource s WHERE s.id = gkey  and invoice = @InvoiceNbr
						SET @table = 'InvoiceStorage'

						-- Sparcsn4
						UPDATE sparcsn4.dbo.argo_chargeable_unit_events SET [status] = 'BILLED',flex_string02 = @InvoiceFinalNumber, flex_date02 = @invoiced
							WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table] = 'S')

					END		
					IF (@invoiceType = 4 or @invoiceType = 6 or @invoiceType = 8)
					BEGIN
						-- Table WarehouseEvents
						INSERT #elemetsSource SELECT ID,[status],'W' FROM WarehouseEvents s WHERE s.invoice = @InvoiceNbr
						UPDATE WarehouseEvents SET [status] = 'BILLED',invoice = @InvoiceFinalNumber where invoice = @InvoiceNbr

						-- Table InvoiceWarehouse
						UPDATE InvoiceWarehouse SET [status] = 'BILLED',invoice = @InvoiceFinalNumber FROM #elemetsSource s WHERE s.id = id  and invoice = @InvoiceNbr
						SET @table = 'InvoiceWarehouse'
					END
					IF(@invoiceType = 5) -- POOL
					BEGIN
						-- Tabela 'PoolStorage'
						INSERT #elemetsSource SELECT ID,[status],'P' FROM poolStorage s WHERE s.invoice = @InvoiceNbr
						UPDATE poolStorage SET [status] = 'BILLED',invoice = @InvoiceFinalNumber where invoice = @InvoiceNbr

						-- Tabele 'InvoicePool' - izvorna kalkulacija
						UPDATE InvoicePool SET [status] = 'BILLED',invoice = @InvoiceFinalNumber FROM #elemetsSource s WHERE s.id = IDpool and invoice = @InvoiceNbr
						SET @table = 'poolStorage'
					END
					IF(@invoiceType = 11) -- Djeljenje troškova za brodove prema servisima
					BEGIN
						-- Table BillingEvents
						INSERT #elemetsSource SELECT gkey,[status],'H' FROM BillingVesselPayers s WHERE s.invoice = @InvoiceNbr

						UPDATE BillingVesselPayers SET  [status] = 'BILLED',invoice = @InvoiceFinalNumber WHERE BillingVesselPayers.invoice = @InvoiceNbr

						-- Table InvoiceEvents
						UPDATE InvoiceEvents SET [status] = 'BILLED',invoice = @InvoiceFinalNumber FROM #elemetsSource s WHERE s.id = gkey  and invoice = @InvoiceNbr
						SET @table = 'InvoiceEvents'
						-- Sparcsn4
						UPDATE sparcsn4.dbo.argo_chargeable_marine_events SET flex_string02 =  CAST(flex_string02 AS varchar(20))+'/'+@InvoiceFinalNumber, flex_date02 = @invoiced
							WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table] = 'H')

					END
					IF (@invoiceType = 31)  -- Skladišnina - kašnjenje broda
					BEGIN
						-- Table BillingEvents/BillingSurcharge
						INSERT #elemetsSource SELECT gkey,[status],'S' FROM BillingStoragePayerDistribution s WHERE s.invoice = @InvoiceNbr
						UPDATE BillingStoragePayerDistribution SET [status] = 'BILLED',invoice = @InvoiceFinalNumber where invoice = @InvoiceNbr
						--UPDATE BillingSurcharge SET [status] = 'BILLED' WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource)

						-- Table InvoiceStorage
						UPDATE InvoiceStorage SET [status] = 'BILLED',invoice = @InvoiceFinalNumber FROM #elemetsSource s WHERE s.id = gkey  and invoice = @InvoiceNbr
						SET @table = 'InvoiceStorage'

						-- Sparcsn4
						--UPDATE sparcsn4.dbo.argo_chargeable_unit_events SET [status] = 'BILLED',flex_string02 = @InvoiceFinalNumber, flex_date02 = @invoiced
						--	WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table] = 'S')

					END

					-- ************** History
					IF @table <> ''
						INSERT changeHistory SELECT 'update',#elemetsSource.id,@table,GETDATE(),'status',[status],'BILLED',@user FROM #elemetsSource
					-- Ako je CANCELLING onda se izvorne stavke moraju vratiti na početak

					IF @BillingCancel is not null
					begin
						-- Ovisno o tipu računa vrši se vračanje statusa stavki za ponovni obračun
						IF (@invoiceType = 1 or @invoiceType = 2 or @invoiceType = 8 or @invoiceType = 9)	-- Brodske operacije/Ostale operacije/Enetgy
						BEGIN
							INSERT #elemetsSource SELECT gkey,[status],CASE WHEN @invoiceType = 9 THEN 'G' ELSE 'E' END FROM BillingEvents s WHERE s.invoice = @BillingCancel
							INSERT #elemetsSource SELECT gkey,[status],'V' FROM BillingVessel s WHERE s.invoice = @BillingCancel

							-- Sparcsn4
							UPDATE sparcsn4.dbo.argo_chargeable_unit_events SET [status] = 'QUEUED',flex_string02 = NULL, flex_date02 = NULL
								WHERE gkey in (SELECT gkey FROM BillingEvents WHERE invoice = @BillingCancel)

							UPDATE sparcsn4.dbo.argo_chargeable_marine_events SET [status] = 'QUEUED',flex_string02 = NULL, flex_date02 = NULL
								WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table]='V')

							-- Table BillingEvents/BillingSurcharge/BillingVessel
							UPDATE BillingEvents SET [status] = 'QUEUED',invoice = NULL, billed = null where invoice = @BillingCancel
							UPDATE BillingSurcharge SET [status] = 'QUEUED' WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table]='E')
							UPDATE BillingVessel SET [status] = 'QUEUED',invoice = NULL, billed = null WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table]='V')

							-- Table InvoiceStorage
							UPDATE InvoiceStorage SET [status] = 'STORNO' WHERE invoice = @BillingCancel
							UPDATE InvoiceStorage SET [status] = 'BILLED',invoice = @InvoiceFinalNumber WHERE invoice = @InvoiceNbr
						END
						IF (@invoiceType = 3)
						BEGIN
							INSERT #elemetsSource SELECT gkey,[status],'S' FROM BillingEvents s WHERE s.invoice = @BillingCancel
							-- Sparcsn4
							UPDATE sparcsn4.dbo.argo_chargeable_unit_events SET [status] = 'QUEUED',flex_string02 = NULL, flex_date02 = NULL
								WHERE gkey in (SELECT gkey FROM BillingEvents WHERE invoice = @BillingCancel)

							-- Table BillingEvents/BillingSurcharge
							UPDATE BillingEvents SET [status] = 'QUEUED',invoice = NULL, billed = null where invoice = @BillingCancel
							UPDATE BillingSurcharge SET [status] = 'QUEUED' WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource)

							-- Table InvoiceStorage
							UPDATE InvoiceStorage SET [status] = 'STORNO' WHERE invoice = @BillingCancel
							UPDATE InvoiceStorage SET [status] = 'BILLED',invoice = @InvoiceFinalNumber WHERE invoice = @InvoiceNbr
						END
						IF (@invoiceType = 4 or @invoiceType = 6 or @invoiceType = 8)
						BEGIN
							--INSERT #elemetsSource SELECT ID,[status],'W' FROM WarehouseEvents s WHERE s.invoice = @BillingCancel
							-- Tabela 'WarehouseEvents'
							UPDATE WarehouseEvents SET [status] = 'QUEUED',invoice = NULL, billed = NULL where invoice = @BillingCancel

							-- Tabele 'InvoiceWarehouse' - izvorna kalkulacija
							UPDATE InvoiceWarehouse SET [status] = 'STORNO' WHERE invoice = @BillingCancel
							UPDATE InvoiceWarehouse SET [status] = 'BILLED',invoice = @InvoiceFinalNumber WHERE invoice = @InvoiceNbr
						END
						IF(@invoiceType = 5) -- POOL
						BEGIN
							--INSERT #elemetsSource SELECT ID,[status],'P' FROM poolStorage s WHERE s.invoice = @BillingCancel
							-- Tabela 'PoolStorage'
							UPDATE poolStorage SET [status] = 'QUEUED',invoice = NULL, billed = NULL where invoice = @BillingCancel

							-- Tabele 'InvoicePool' - izvorna kalkulacija
							UPDATE InvoicePool SET [status] = 'STORNO' WHERE invoice = @BillingCancel
							UPDATE InvoicePool SET [status] = 'BILLED',invoice = @InvoiceFinalNumber WHERE invoice = @InvoiceNbr
						END
						IF(@invoiceType = 11) -- Djeljenje troškova za brodove prema servisima
						BEGIN
							INSERT #elemetsSource SELECT gkey,[status],'H' FROM BillingVesselPayers s WHERE s.invoice = @BillingCancel

							-- Table BillingEvents/BillingSurcharge/BillingVessel
							UPDATE BillingVesselPayers SET [status] = 'QUEUED',invoice = NULL, billed = null WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table]='H')

							-- Table InvoiceStorage
							UPDATE InvoiceStorage SET [status] = 'STORNO' WHERE invoice = @BillingCancel
							UPDATE InvoiceStorage SET [status] = 'BILLED',invoice = @InvoiceFinalNumber WHERE invoice = @InvoiceNbr

						END

						IF(@invoiceType = 31) -- Skladišnina - kašnjenje broda
						BEGIN
							INSERT #elemetsSource SELECT gkey,[status],'S' FROM BillingStoragePayerDistribution s WHERE s.invoice = @BillingCancel
							-- Sparcsn4
							--UPDATE sparcsn4.dbo.argo_chargeable_unit_events SET [status] = 'QUEUED',flex_string02 = NULL, flex_date02 = NULL
							--	WHERE gkey in (SELECT gkey FROM BillingStoragePayerDistribution WHERE invoice = @BillingCancel)

							-- Table BillingEvents/BillingSurcharge
							UPDATE BillingStoragePayerDistribution SET [status] = 'QUEUED',invoice = NULL, billed = null where invoice = @BillingCancel
							UPDATE BillingSurcharge SET [status] = 'QUEUED' WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource)

							-- Table InvoiceStorage
							UPDATE InvoiceStorage SET [status] = 'STORNO' WHERE invoice = @BillingCancel
							UPDATE InvoiceStorage SET [status] = 'BILLED',invoice = @InvoiceFinalNumber WHERE invoice = @InvoiceNbr
						END
						-- Status računa u tabeli 'Invoices'
						UPDATE Invoices SET InvoiceStatus = 'STORNO',@BillingCancelId = ID WHERE InvoiceNumber = @BillingCancel
						-- Hystory
						INSERT changeHistory SELECT 'update',@BillingCancelId,'Invoices',GETDATE(),'status','BILLED','STORNO',@user

						-- Pvo pronađi ExtractStatus u računu koji se stornira
						SELECT REPLACE(AltInvoiceNumber,@BillingCancel, @InvoiceFinalNumber) as AltInvoiceNumber INTO #TMP
							FROM SAPInvoices WHERE InvoiceNumber = @BillingCancel and ExtractStatus in ('1','3') and SAPinvoice is null

						UPDATE SAPInvoices SET ExtractStatus = '6',Changed=GETDATE() WHERE AltInvoiceNumber IN (SELECT AltInvoiceNumber FROM #TMP)

									-- ************** History
						IF @table <> ''
							INSERT changeHistory SELECT 'update',#elemetsSource.id,@table,GETDATE(),'status',[status],'BILLED',@user FROM #elemetsSource

					end

					DROP table #SAPinvoiceId
					DROP TABLE #itemsId
					DROP TABLE #invoiceElemets
					DROP TABLE #elemetsSource
					COMMIT TRANSACTION
					SELECT @InvoiceFinalNumber AS InvoiceNumber, @FiscalizationNumber as FiscalizationNumber
				END TRY
				BEGIN CATCH
					exec intf_ShowError
					ROLLBACK TRANSACTION
				END CATCH
			END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
			@"
			USE [ICTSIbilling]
			GO
			/****** Object:  StoredProcedure [dbo].[inv_ApprovingInvoice]    Script Date: 07/10/2025 10:05:21 ******/
			SET ANSI_NULLS ON
			GO
			SET QUOTED_IDENTIFIER ON
			GO
			-- =============================================
			-- Author:		Josip Kubasa
			-- Create date: 07.08.2014
			-- Description:
			-- =============================================
			ALTER PROCEDURE [dbo].[inv_ApprovingInvoice]
				@InvoiceNbr AS NVARCHAR(30),
				@user AS NVARCHAR(20)
			AS
			BEGIN
				DECLARE @BillingCancel NVARCHAR(10),
					@BillingCancelId as int,
					@InvoiceFinalNumber NVARCHAR(25),
					@FiscalizationNumber NVARCHAR(25),
					@invoiceType as int,
					@invoiceID AS int,
					@table as varchar(20),
					@invoiced as smallDateTime	   

				-- Inicijalni podaci
				SELECT @invoiceID = ID, @invoiceType = billing_method, @BillingCancel = [CancelledBillingNumber], @invoiced = Invoiced FROM [ICTSIbilling].[dbo].[Invoices] where InvoiceNumber = @InvoiceNbr
				SET @table = ''

				-- Novi pravi broj računa
				SELECT @InvoiceFinalNumber = dbo.NewInvoiceNumber()

				-- Fiskalni broj računa
				SELECT @FiscalizationNumber = dbo.NewFiscalization()


				BEGIN TRANSACTION
				BEGIN TRY

					-- KreirajTMP tabele
					CREATE TABLE #SAPinvoiceId (id int,altInvoiceNbr nvarchar(30))
					CREATE TABLE #itemsId (id int,SAPid int)
					CREATE TABLE #invoiceElemets (id int)
					CREATE TABLE #elemetsSource (id int,[status] nvarchar(20),[table] char(1))
					CREATE TABLE #invoiceDetails(id int)

					-- *********** Zapamti izvorne podatke
					-- Zapamti sve zapise za račun
					INSERT #SAPinvoiceId SELECT ID,@InvoiceFinalNumber+DistrChanel+Division FROM SAPInvoices WHERE InvoiceNumber = @InvoiceNbr
					INSERT #itemsId select i.ID,s.ID FROM InvoiceItems i,SAPInvoices s where s.InvoiceNumber = @InvoiceNbr and s.AltInvoiceNumber = i.AltInvoiceNumber
					IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'InvoiceDetails')
					BEGIN
						INSERT #invoiceDetails select Id from InvoiceDetails where InvoiceNumber = @InvoiceNbr
					END
					-- setiraj brojeve računa na NULL
					-- InvoiceItems
					UPDATE InvoiceItems SET AltInvoiceNumber = NULL where ID in (select id from #itemsId)
					-- SAPinvoice
					UPDATE SAPInvoices SET InvoiceNumber = null,
					AltInvoiceNumber = CAST(@InvoiceFinalNumber AS NVARCHAR(30))+DistrChanel+Division,
					Changed=GETDATE()
					WHERE ID in  (SELECT ID FROM #SAPinvoiceId)

					IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'InvoiceDetails')
					BEGIN
						update InvoiceDetails SET InvoiceNumber = null
						where Id in (select id from #invoiceDetails)
					END

					-- ******** Upiši novi broj računa
					-- Invoice
					UPDATE Invoices SET InvoiceNumber = @InvoiceFinalNumber, Fiscalization = @FiscalizationNumber,InvoiceStatus = 'BILLED',Approved = GETDATE(),ApprovedUser = @user WHERE InvoiceNumber = @InvoiceNbr
					UPDATE Invoices SET CustomerRefNbr = @InvoiceFinalNumber where CustomerRefNbr = @InvoiceNbr
					-- InvoiceAdded
					UPDATE InvoiceAdded SET InvoiceNumber = @InvoiceFinalNumber WHERE InvoiceNumber = @InvoiceNbr
					-- SAPinvoices
					UPDATE SAPInvoices SET InvoiceNumber = @InvoiceFinalNumber,ExtractStatus = '1',Changed=GETDATE() WHERE ID IN (SELECT ID FROM #SAPinvoiceId)

					-- InvoiceItems
					UPDATE InvoiceItems set AltInvoiceNumber = s.altInvoiceNbr FROM #itemsId i, #SAPinvoiceId s where i.id = InvoiceItems.ID  AND s.id = i.SAPid

					IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'InvoiceDetails')
					BEGIN
						update InvoiceDetails SET InvoiceNumber = @InvoiceFinalNumber
						where id in (select id from #invoiceDetails)
					END

					-- Ako je račun na kojemu su spajani ostali računi mora se u izvornom računu promjeniti broj računa u polju 'CancelledBillingInvoice'
					UPDATE Invoices SET CancelledBillingNumber = @InvoiceFinalNumber WHERE InvoiceStatus = 'JOINTED' and CancelledBillingNumber = @InvoiceNbr

					-- ********* Potrebno ažurirati sve ostale izvorne tabele
					-- Ovisno o @invoiceType definira se slijed računa za promjenu
					IF (@invoiceType = 1 or @invoiceType = 2 or @invoiceType = 8 or @invoiceType = 9)	-- Brodske operacije/Ostale operacije/energija
					BEGIN
						-- Table BillingEvents/BillingSurcharge
						INSERT #elemetsSource SELECT gkey,[status],CASE WHEN @invoiceType = 9 THEN 'G' ELSE 'E' END FROM BillingEvents s WHERE s.invoice = @InvoiceNbr
						INSERT #elemetsSource SELECT gkey,[status],'V' FROM BillingVessel s WHERE s.invoice = @InvoiceNbr
						UPDATE BillingEvents SET [status] = 'BILLED',invoice = @InvoiceFinalNumber where invoice = @InvoiceNbr
						UPDATE BillingSurcharge SET [status] = 'BILLED' WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table] = 'E')
						UPDATE BillingVessel SET  [status] = 'BILLED',invoice = @InvoiceFinalNumber WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table] = 'V')

						-- Table InvoiceEvents
						UPDATE InvoiceEvents SET [status] = 'BILLED',invoice = @InvoiceFinalNumber FROM #elemetsSource s WHERE s.id = gkey  and invoice = @InvoiceNbr
						SET @table = 'InvoiceEvents'

						-- Sparcsn4
						UPDATE sparcsn4.dbo.argo_chargeable_unit_events SET [status] = 'BILLED',flex_string02 = @InvoiceFinalNumber, flex_date02 = @invoiced
							WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table] = 'E')
						UPDATE sparcsn4.dbo.argo_chargeable_marine_events SET [status] = 'BILLED',flex_string02 = @InvoiceFinalNumber, flex_date02 = @invoiced
							WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table] = 'V')

					END
					IF (@invoiceType = 3)
					BEGIN
						-- Table BillingEvents/BillingSurcharge
						INSERT #elemetsSource SELECT gkey,[status],'S' FROM BillingEvents s WHERE s.invoice = @InvoiceNbr
						UPDATE BillingEvents SET [status] = 'BILLED',invoice = @InvoiceFinalNumber where invoice = @InvoiceNbr
						UPDATE BillingSurcharge SET [status] = 'BILLED' WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource)

						-- Table InvoiceStorage
						UPDATE InvoiceStorage SET [status] = 'BILLED',invoice = @InvoiceFinalNumber FROM #elemetsSource s WHERE s.id = gkey  and invoice = @InvoiceNbr
						SET @table = 'InvoiceStorage'

						-- Sparcsn4
						UPDATE sparcsn4.dbo.argo_chargeable_unit_events SET [status] = 'BILLED',flex_string02 = @InvoiceFinalNumber, flex_date02 = @invoiced
							WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table] = 'S')

					END		
					IF (@invoiceType = 4 or @invoiceType = 6 or @invoiceType = 8)
					BEGIN
						-- Table WarehouseEvents
						INSERT #elemetsSource SELECT ID,[status],'W' FROM WarehouseEvents s WHERE s.invoice = @InvoiceNbr
						UPDATE WarehouseEvents SET [status] = 'BILLED',invoice = @InvoiceFinalNumber where invoice = @InvoiceNbr

						-- Table InvoiceWarehouse
						UPDATE InvoiceWarehouse SET [status] = 'BILLED',invoice = @InvoiceFinalNumber FROM #elemetsSource s WHERE s.id = id  and invoice = @InvoiceNbr
						SET @table = 'InvoiceWarehouse'
					END
					IF(@invoiceType = 5) -- POOL
					BEGIN
						-- Tabela 'PoolStorage'
						INSERT #elemetsSource SELECT ID,[status],'P' FROM poolStorage s WHERE s.invoice = @InvoiceNbr
						UPDATE poolStorage SET [status] = 'BILLED',invoice = @InvoiceFinalNumber where invoice = @InvoiceNbr

						-- Tabele 'InvoicePool' - izvorna kalkulacija
						UPDATE InvoicePool SET [status] = 'BILLED',invoice = @InvoiceFinalNumber FROM #elemetsSource s WHERE s.id = IDpool and invoice = @InvoiceNbr
						SET @table = 'poolStorage'
					END
					IF(@invoiceType = 11) -- Djeljenje troškova za brodove prema servisima
					BEGIN
						-- Table BillingEvents
						INSERT #elemetsSource SELECT gkey,[status],'H' FROM BillingVesselPayers s WHERE s.invoice = @InvoiceNbr

						UPDATE BillingVesselPayers SET  [status] = 'BILLED',invoice = @InvoiceFinalNumber WHERE BillingVesselPayers.invoice = @InvoiceNbr

						-- Table InvoiceEvents
						UPDATE InvoiceEvents SET [status] = 'BILLED',invoice = @InvoiceFinalNumber FROM #elemetsSource s WHERE s.id = gkey  and invoice = @InvoiceNbr
						SET @table = 'InvoiceEvents'
						-- Sparcsn4
						UPDATE sparcsn4.dbo.argo_chargeable_marine_events SET flex_string02 =  CAST(flex_string02 AS varchar(20))+'/'+@InvoiceFinalNumber, flex_date02 = @invoiced
							WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table] = 'H')

					END
					IF (@invoiceType = 31)  -- Skladišnina - kašnjenje broda
					BEGIN
						-- Table BillingEvents/BillingSurcharge
						INSERT #elemetsSource SELECT gkey,[status],'S' FROM BillingStoragePayerDistribution s WHERE s.invoice = @InvoiceNbr
						UPDATE BillingStoragePayerDistribution SET [status] = 'BILLED',invoice = @InvoiceFinalNumber where invoice = @InvoiceNbr
						--UPDATE BillingSurcharge SET [status] = 'BILLED' WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource)

						-- Table InvoiceStorage
						UPDATE InvoiceStorage SET [status] = 'BILLED',invoice = @InvoiceFinalNumber FROM #elemetsSource s WHERE s.id = gkey  and invoice = @InvoiceNbr
						SET @table = 'InvoiceStorage'

						-- Sparcsn4
						--UPDATE sparcsn4.dbo.argo_chargeable_unit_events SET [status] = 'BILLED',flex_string02 = @InvoiceFinalNumber, flex_date02 = @invoiced
						--	WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table] = 'S')

					END

					-- ************** History
					IF @table <> ''
						INSERT changeHistory SELECT 'update',#elemetsSource.id,@table,GETDATE(),'status',[status],'BILLED',@user FROM #elemetsSource
					-- Ako je CANCELLING onda se izvorne stavke moraju vratiti na početak

					IF @BillingCancel is not null
					begin
						-- Ovisno o tipu računa vrši se vračanje statusa stavki za ponovni obračun
						IF (@invoiceType = 1 or @invoiceType = 2 or @invoiceType = 8 or @invoiceType = 9)	-- Brodske operacije/Ostale operacije/Enetgy
						BEGIN
							INSERT #elemetsSource SELECT gkey,[status],CASE WHEN @invoiceType = 9 THEN 'G' ELSE 'E' END FROM BillingEvents s WHERE s.invoice = @BillingCancel
							INSERT #elemetsSource SELECT gkey,[status],'V' FROM BillingVessel s WHERE s.invoice = @BillingCancel

							-- Sparcsn4
							UPDATE sparcsn4.dbo.argo_chargeable_unit_events SET [status] = 'QUEUED',flex_string02 = NULL, flex_date02 = NULL
								WHERE gkey in (SELECT gkey FROM BillingEvents WHERE invoice = @BillingCancel)

							UPDATE sparcsn4.dbo.argo_chargeable_marine_events SET [status] = 'QUEUED',flex_string02 = NULL, flex_date02 = NULL
								WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table]='V')

							-- Table BillingEvents/BillingSurcharge/BillingVessel
							UPDATE BillingEvents SET [status] = 'QUEUED',invoice = NULL, billed = null where invoice = @BillingCancel
							UPDATE BillingSurcharge SET [status] = 'QUEUED' WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table]='E')
							UPDATE BillingVessel SET [status] = 'QUEUED',invoice = NULL, billed = null WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table]='V')

							-- Table InvoiceStorage
							UPDATE InvoiceStorage SET [status] = 'STORNO' WHERE invoice = @BillingCancel
							UPDATE InvoiceStorage SET [status] = 'BILLED',invoice = @InvoiceFinalNumber WHERE invoice = @InvoiceNbr
						END
						IF (@invoiceType = 3)
						BEGIN
							INSERT #elemetsSource SELECT gkey,[status],'S' FROM BillingEvents s WHERE s.invoice = @BillingCancel
							-- Sparcsn4
							UPDATE sparcsn4.dbo.argo_chargeable_unit_events SET [status] = 'QUEUED',flex_string02 = NULL, flex_date02 = NULL
								WHERE gkey in (SELECT gkey FROM BillingEvents WHERE invoice = @BillingCancel)

							-- Table BillingEvents/BillingSurcharge
							UPDATE BillingEvents SET [status] = 'QUEUED',invoice = NULL, billed = null where invoice = @BillingCancel
							UPDATE BillingSurcharge SET [status] = 'QUEUED' WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource)

							-- Table InvoiceStorage
							UPDATE InvoiceStorage SET [status] = 'STORNO' WHERE invoice = @BillingCancel
							UPDATE InvoiceStorage SET [status] = 'BILLED',invoice = @InvoiceFinalNumber WHERE invoice = @InvoiceNbr
						END
						IF (@invoiceType = 4 or @invoiceType = 6 or @invoiceType = 8)
						BEGIN
							--INSERT #elemetsSource SELECT ID,[status],'W' FROM WarehouseEvents s WHERE s.invoice = @BillingCancel
							-- Tabela 'WarehouseEvents'
							UPDATE WarehouseEvents SET [status] = 'QUEUED',invoice = NULL, billed = NULL where invoice = @BillingCancel

							-- Tabele 'InvoiceWarehouse' - izvorna kalkulacija
							UPDATE InvoiceWarehouse SET [status] = 'STORNO' WHERE invoice = @BillingCancel
							UPDATE InvoiceWarehouse SET [status] = 'BILLED',invoice = @InvoiceFinalNumber WHERE invoice = @InvoiceNbr
						END
						IF(@invoiceType = 5) -- POOL
						BEGIN
							--INSERT #elemetsSource SELECT ID,[status],'P' FROM poolStorage s WHERE s.invoice = @BillingCancel
							-- Tabela 'PoolStorage'
							UPDATE poolStorage SET [status] = 'QUEUED',invoice = NULL, billed = NULL where invoice = @BillingCancel

							-- Tabele 'InvoicePool' - izvorna kalkulacija
							UPDATE InvoicePool SET [status] = 'STORNO' WHERE invoice = @BillingCancel
							UPDATE InvoicePool SET [status] = 'BILLED',invoice = @InvoiceFinalNumber WHERE invoice = @InvoiceNbr
						END
						IF(@invoiceType = 11) -- Djeljenje troškova za brodove prema servisima
						BEGIN
							INSERT #elemetsSource SELECT gkey,[status],'H' FROM BillingVesselPayers s WHERE s.invoice = @BillingCancel

							-- Table BillingEvents/BillingSurcharge/BillingVessel
							UPDATE BillingVesselPayers SET [status] = 'QUEUED',invoice = NULL, billed = null WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource WHERE [table]='H')

							-- Table InvoiceStorage
							UPDATE InvoiceStorage SET [status] = 'STORNO' WHERE invoice = @BillingCancel
							UPDATE InvoiceStorage SET [status] = 'BILLED',invoice = @InvoiceFinalNumber WHERE invoice = @InvoiceNbr

						END

						IF(@invoiceType = 31) -- Skladišnina - kašnjenje broda
						BEGIN
							INSERT #elemetsSource SELECT gkey,[status],'S' FROM BillingStoragePayerDistribution s WHERE s.invoice = @BillingCancel
							-- Sparcsn4
							--UPDATE sparcsn4.dbo.argo_chargeable_unit_events SET [status] = 'QUEUED',flex_string02 = NULL, flex_date02 = NULL
							--	WHERE gkey in (SELECT gkey FROM BillingStoragePayerDistribution WHERE invoice = @BillingCancel)

							-- Table BillingEvents/BillingSurcharge
							UPDATE BillingStoragePayerDistribution SET [status] = 'QUEUED',invoice = NULL, billed = null where invoice = @BillingCancel
							UPDATE BillingSurcharge SET [status] = 'QUEUED' WHERE gkey in (SELECT #elemetsSource.id FROM #elemetsSource)

							-- Table InvoiceStorage
							UPDATE InvoiceStorage SET [status] = 'STORNO' WHERE invoice = @BillingCancel
							UPDATE InvoiceStorage SET [status] = 'BILLED',invoice = @InvoiceFinalNumber WHERE invoice = @InvoiceNbr
						END
						-- Status računa u tabeli 'Invoices'
						UPDATE Invoices SET InvoiceStatus = 'STORNO',@BillingCancelId = ID WHERE InvoiceNumber = @BillingCancel
						-- Hystory
						INSERT changeHistory SELECT 'update',@BillingCancelId,'Invoices',GETDATE(),'status','BILLED','STORNO',@user

						-- Pvo pronađi ExtractStatus u računu koji se stornira
						SELECT REPLACE(AltInvoiceNumber,@BillingCancel, @InvoiceFinalNumber) as AltInvoiceNumber INTO #TMP
							FROM SAPInvoices WHERE InvoiceNumber = @BillingCancel and ExtractStatus in ('1','3') and SAPinvoice is null

						UPDATE SAPInvoices SET ExtractStatus = '6',Changed=GETDATE() WHERE AltInvoiceNumber IN (SELECT AltInvoiceNumber FROM #TMP)

									-- ************** History
						IF @table <> ''
							INSERT changeHistory SELECT 'update',#elemetsSource.id,@table,GETDATE(),'status',[status],'BILLED',@user FROM #elemetsSource

					end

					DROP table #SAPinvoiceId
					DROP TABLE #itemsId
					DROP TABLE #invoiceElemets
					DROP TABLE #elemetsSource
					COMMIT TRANSACTION
					SELECT @InvoiceFinalNumber AS InvoiceNumber, @FiscalizationNumber as FiscalizationNumber
				END TRY
				BEGIN CATCH
					exec intf_ShowError
					ROLLBACK TRANSACTION
				END CATCH
			END");

        }
    }
}
