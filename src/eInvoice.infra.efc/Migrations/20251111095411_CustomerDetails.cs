using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eInvoice.infra.efc.Migrations
{
    /// <inheritdoc />
    public partial class CustomerDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    IsRegistered = table.Column<bool>(type: "bit", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    Updated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerDetails", x => x.Id);
                });

            migrationBuilder.Sql("""
                CREATE PROCEDURE inv_InsertOrUpdateCustomerDetails
                    @CompanyId nvarchar(35),
                    @operator NVARCHAR(30) = NULL
                AS
                BEGIN
                    SET NOCOUNT ON;

                	declare @Id int;

                    if not exists (select 1 from CustomerDetails where CompanyId = @CompanyId)
                    begin
                        INSERT CustomerDetails (CompanyId, Updated)
                        VALUES (@CompanyId, SYSDATETIMEOFFSET());

                		SET @Id = SCOPE_IDENTITY();
                    end
                    else 
                    begin 
                        UPDATE CustomerDetails SET 
                            IsRegistered = null,
                            Updated = SYSDATETIMEOFFSET()
                        WHERE CompanyId = @CompanyId

                		Select @Id = id from CustomerDetails where CompanyId = @CompanyId
                    end

                    insert into changeHistory ([Description], gkey, [table], [field], oldValue, newValue, [user]) 
                    values ('update', @Id, 'CustomerDetails', getdate(), 'IsRegistered', null, @operator)
                END
                """);

            migrationBuilder.Sql("""
                USE [ICTSIbilling]
                GO
                /****** Object:  StoredProcedure [dbo].[master_GetCustomers]    Script Date: 11/11/2025 13:41:15 ******/
                SET ANSI_NULLS ON
                GO
                SET QUOTED_IDENTIFIER ON
                GO
                -- =============================================
                -- Author:		Josip Kubasa
                -- Create date: 13.05.2014
                -- Description:	Get customers data
                -- Last Change: 20.5.2014, Goran Sokolovic
                --    -||-    : 11.07.2014 Vladimir Travalja	 
                -- =============================================
                ALTER PROCEDURE [dbo].[master_GetCustomers]
                (
                @FieldName as nVarChar(20) = null,
                @FieldValue as nVarChar(35) = null
                )
                AS
                BEGIN
                	SET NOCOUNT ON;

                	IF @FieldValue is null
                		BEGIN
                			SELECT a.CustomerNumber, Name1,a.DistributionChannel,a.Division, Name2, Name3, Name4, Street2, HouseNumber, CityPostalCode, a.City City, CityCountryKey, LanguageKey, FirstTelephone, FirstMobile, FirstFax, EMailAddress,
                				TaxNumber1, TaxNumber2, ContactPerson,CountryCode 	--ContactCountryKey, 
                				TermsOfPaymentKey, CustomersCurrency, BankAccount, RecordStatus, --Updated, 
                				dbo.LineOperators.id LineOperatorID, dbo.LineOperators.name LineOperatorName, a.ID
                                ,cd.IsRegistered IsRegistered
                			FROM dbo.LineOperators
                				INNER JOIN DBO.PayerSAPmap on dbo.LineOperators.id = dbo.PayerSAPmap.NavisPayer
                				RIGHT OUTER JOIN dbo.CustomerMaster a on dbo.PayerSAPmap.SAPpayer = a.CustomerNumber
                                LEFT JOIN CustomerDetails cd on a.TaxNumber2 = cd.CompanyId
                			ORDER BY Name1
                		END
                	ELSE
                		BEGIN
                		-- V.T. 11.07.2014 izmjenjen select kako bi i kod filtriranja mogao dohvatiti LineOperatorID i LineOperatorName 
                			SELECT a.CustomerNumber, Name1,a.DistributionChannel,a.Division, Name2, Name3, Name4, Street2, HouseNumber, CityPostalCode, a.City City, CityCountryKey, LanguageKey, FirstTelephone, FirstMobile, FirstFax, EMailAddress,
                				TaxNumber1, TaxNumber2, ContactPerson, CountryCode	--ContactCountryKey, 
                				TermsOfPaymentKey, CustomersCurrency, BankAccount, RecordStatus, --Updated, 
                				dbo.LineOperators.id LineOperatorID, dbo.LineOperators.name LineOperatorName, a.ID
                                ,cd.IsRegistered IsRegistered
                			FROM dbo.LineOperators
                				INNER JOIN DBO.PayerSAPmap on dbo.LineOperators.id = dbo.PayerSAPmap.NavisPayer
                				RIGHT OUTER JOIN dbo.CustomerMaster a on dbo.PayerSAPmap.SAPpayer = a.CustomerNumber
                                LEFT JOIN CustomerDetails cd on a.TaxNumber2 = cd.CompanyId
                			WHERE case @FieldName WHEN 'Name1' THEN Name1 WHEN 'Street2' THEN Street2 WHEN 'CityPostalCode' THEN CityPostalCode WHEN 'City' THEN A.City END like '%' + @FieldValue + '%'
                			ORDER BY Name1
                		END
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS inv_InsertOrUpdateCustomerDetails");

            migrationBuilder.DropTable(
                name: "CustomerDetails");

            migrationBuilder.Sql("""
                USE [ICTSIbilling]
                GO
                /****** Object:  StoredProcedure [dbo].[master_GetCustomers]    Script Date: 11/11/2025 13:41:15 ******/
                SET ANSI_NULLS ON
                GO
                SET QUOTED_IDENTIFIER ON
                GO
                -- =============================================
                -- Author:		Josip Kubasa
                -- Create date: 13.05.2014
                -- Description:	Get customers data
                -- Last Change: 20.5.2014, Goran Sokolovic
                --    -||-    : 11.07.2014 Vladimir Travalja	 
                -- =============================================
                ALTER PROCEDURE [dbo].[master_GetCustomers]
                (
                @FieldName as nVarChar(20) = null,
                @FieldValue as nVarChar(35) = null
                )
                AS
                BEGIN
                	SET NOCOUNT ON;

                	IF @FieldValue is null
                		BEGIN
                			SELECT a.CustomerNumber, Name1,a.DistributionChannel,a.Division, Name2, Name3, Name4, Street2, HouseNumber, CityPostalCode, a.City City, CityCountryKey, LanguageKey, FirstTelephone, FirstMobile, FirstFax, EMailAddress,
                				TaxNumber1, TaxNumber2, ContactPerson,CountryCode 	--ContactCountryKey, 
                				TermsOfPaymentKey, CustomersCurrency, BankAccount, RecordStatus, --Updated, 
                				dbo.LineOperators.id LineOperatorID, dbo.LineOperators.name LineOperatorName, a.ID
                			FROM dbo.LineOperators
                				INNER JOIN DBO.PayerSAPmap on dbo.LineOperators.id = dbo.PayerSAPmap.NavisPayer
                				RIGHT OUTER JOIN dbo.CustomerMaster a on dbo.PayerSAPmap.SAPpayer = a.CustomerNumber
                			ORDER BY Name1
                		END
                	ELSE
                		BEGIN
                		-- V.T. 11.07.2014 izmjenjen select kako bi i kod filtriranja mogao dohvatiti LineOperatorID i LineOperatorName 
                			SELECT a.CustomerNumber, Name1,a.DistributionChannel,a.Division, Name2, Name3, Name4, Street2, HouseNumber, CityPostalCode, a.City City, CityCountryKey, LanguageKey, FirstTelephone, FirstMobile, FirstFax, EMailAddress,
                				TaxNumber1, TaxNumber2, ContactPerson, CountryCode	--ContactCountryKey, 
                				TermsOfPaymentKey, CustomersCurrency, BankAccount, RecordStatus, --Updated, 
                				dbo.LineOperators.id LineOperatorID, dbo.LineOperators.name LineOperatorName, a.ID
                			FROM dbo.LineOperators
                				INNER JOIN DBO.PayerSAPmap on dbo.LineOperators.id = dbo.PayerSAPmap.NavisPayer
                				RIGHT OUTER JOIN dbo.CustomerMaster a on dbo.PayerSAPmap.SAPpayer = a.CustomerNumber
                			WHERE case @FieldName WHEN 'Name1' THEN Name1 WHEN 'Street2' THEN Street2 WHEN 'CityPostalCode' THEN CityPostalCode WHEN 'City' THEN A.City END like '%' + @FieldValue + '%'
                			ORDER BY Name1
                		END
                END
                """);
        }
    }
}
