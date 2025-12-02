using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eInvoice.infra.efc.Migrations
{
    /// <inheritdoc />
    public partial class InitScript : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessSpaceArea",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessSpaceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessSpaceCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessSpaceArea", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaxNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfigName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConfigValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SalesOrganization = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DistributionChannel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Division = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerAccountGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SearchTerm1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SearchTerm2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Street2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HouseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CityPostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CityCountryKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LanguageKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstTelephone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstMobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstFax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxNumber1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxNumber2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VatregNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NatPerson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IndustryKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerClass = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IndustryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TermsOfPaymentKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalesDistrict = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalesOffice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalesGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerGroup = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomersCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditControlArea = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoldToParty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Consignee = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BillToParty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Payer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShipToParty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankCountryKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankAccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReconciliationAccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterestIndicator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentBlock = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HouseBank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LockBox = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DunningProcedure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DunningBlock = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DunningLevel = table.Column<int>(type: "int", nullable: true),
                    WithholdingTaxCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WithholdingTaxIndicator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IndividualLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RecordStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerMaster", x => x.Id);
                    table.UniqueConstraint("AK_CustomerMaster_CustomerNumber", x => x.CustomerNumber);
                });

            migrationBuilder.CreateTable(
                name: "EdiinvoicingTo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillToParty = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Service = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    User = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastSending = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChecking = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StreetName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BuildingNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalZone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoldToParty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StpName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AltMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AltAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AltUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AltPassword = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EdiinvoicingTo", x => x.Id);
                    table.UniqueConstraint("AK_EdiinvoicingTo_BillToParty", x => x.BillToParty);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Gkey = table.Column<long>(type: "bigint", nullable: false),
                    Container = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Invoice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Invoiced = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Teu = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Cat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Imo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rf = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Oog = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tariff = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Event = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescryptionHr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VesselId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VesselName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VeselVisitId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LineOperatorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Measure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    User = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Item = table.Column<int>(type: "int", nullable: true),
                    Table = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayeeCustomerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Percentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoicePools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Item = table.Column<int>(type: "int", nullable: true),
                    Idpool = table.Column<int>(type: "int", nullable: true),
                    LineOperatorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Containers20 = table.Column<short>(type: "smallint", nullable: true),
                    Containers40 = table.Column<short>(type: "smallint", nullable: true),
                    Containers45 = table.Column<short>(type: "smallint", nullable: true),
                    PoolTeus = table.Column<short>(type: "smallint", nullable: true),
                    TotalTeus = table.Column<short>(type: "smallint", nullable: true),
                    Storage = table.Column<short>(type: "smallint", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Select = table.Column<int>(type: "int", nullable: false),
                    Invoice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Measure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    User = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tariff = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Invoiced = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PayeeCustomerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceStorages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Item = table.Column<int>(type: "int", nullable: true),
                    Container = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Invoice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Carr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Imo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rf = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Oog = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StorageTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    JobTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Days = table.Column<short>(type: "smallint", nullable: true),
                    Free = table.Column<int>(type: "int", nullable: true),
                    Teu = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TariffDays1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Days1 = table.Column<short>(type: "smallint", nullable: true),
                    TotDays1 = table.Column<int>(type: "int", nullable: false),
                    TariffDays2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Days2 = table.Column<short>(type: "smallint", nullable: true),
                    TotDays2 = table.Column<int>(type: "int", nullable: false),
                    TariffDays3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Days3 = table.Column<short>(type: "smallint", nullable: true),
                    TotDays3 = table.Column<int>(type: "int", nullable: false),
                    TariffDays4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Days4 = table.Column<short>(type: "smallint", nullable: true),
                    TotDays4 = table.Column<int>(type: "int", nullable: false),
                    Tariff = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OnTerminal = table.Column<int>(type: "int", nullable: true),
                    Event = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionHr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LineOperatorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VesselId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VesselName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VeselVisitId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Measure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gkey = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    User = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Table = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Invoiced = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PayeeCustomerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContractId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Eesgenerated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceStorages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TariffDescs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tariff = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Event = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TariffDescs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tariffs",
                columns: table => new
                {
                    Tariff = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    Event = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Surcharge = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsF = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Truck = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Units = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsImport = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsExport = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsTranship = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hazardous = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Oversize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Teu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GroupDays = table.Column<int>(type: "int", nullable: true),
                    billing_method = table.Column<short>(type: "smallint", nullable: true),
                    EventGroup = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SapdistChannel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sapdivision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsStorage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OogEvent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImoEvent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Changed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VissibleInWh = table.Column<bool>(type: "bit", nullable: true),
                    EES_surcharge = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tariffs1", x => new { x.Tariff, x.Event });
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    UserPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserActiv = table.Column<bool>(type: "bit", nullable: true),
                    UserCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserCreator = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserChanged = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserChanger = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_User_User_UserChanger",
                        column: x => x.UserChanger,
                        principalTable: "User",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_User_User_UserCreator",
                        column: x => x.UserCreator,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Vatclause",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceTextAlt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceTextLoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Changed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Valid = table.Column<bool>(type: "bit", nullable: true),
                    CustomerNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vatclause", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VslVoyages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Gkey = table.Column<long>(type: "bigint", nullable: false),
                    VesselId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomsId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CarrierMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VisitNbr = table.Column<long>(type: "bigint", nullable: true),
                    Phase = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperatorGkey = table.Column<long>(type: "bigint", nullable: true),
                    CpxGkey = table.Column<long>(type: "bigint", nullable: false),
                    FcyGkey = table.Column<long>(type: "bigint", nullable: true),
                    NextFcyGkey = table.Column<long>(type: "bigint", nullable: true),
                    Ata = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Atd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CvcvdGkey = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IbVyg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ObVyg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Service = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeOffManifest = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartWork = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndWork = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CarrierGkey = table.Column<long>(type: "bigint", nullable: false),
                    RadioCallSign = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VslVoyages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Swiftid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ibanid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankData_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceTypeEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceTypeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InvoiceDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Commentary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VatClause = table.Column<int>(type: "int", nullable: true),
                    SapbillingType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceDescriptionLocal = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceTypeEntity", x => x.Id);
                    table.UniqueConstraint("AK_InvoiceTypeEntity_InvoiceTypeId", x => x.InvoiceTypeId);
                    table.ForeignKey(
                        name: "FK_InvoiceTypeEntity_Vatclause_VatClause",
                        column: x => x.VatClause,
                        principalTable: "Vatclause",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CancelledBillingNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillingMethod = table.Column<short>(type: "smallint", nullable: true),
                    BillOfLading = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceType = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    BillingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BaselineDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalesGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalesDistrict = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoldToParty = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Payer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillToParty = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShipToParty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Consignee = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ponumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TermsOfPaymentKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SapbillingType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DatePeriodFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DatePeriodTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Invoiced = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InvoicedUser = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Controlled = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ControlledUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Approved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extracted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExtractStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fiscalization = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VesselName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VesseVisitlId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerRefNbr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ratio = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rejected = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxClass = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.UniqueConstraint("AK_Invoices_InvoiceNumber", x => x.InvoiceNumber);
                    table.ForeignKey(
                        name: "FK_Invoices_CustomerMaster_BillToParty",
                        column: x => x.BillToParty,
                        principalTable: "CustomerMaster",
                        principalColumn: "CustomerNumber");
                    table.ForeignKey(
                        name: "FK_Invoices_CustomerMaster_SoldToParty",
                        column: x => x.SoldToParty,
                        principalTable: "CustomerMaster",
                        principalColumn: "CustomerNumber");
                    table.ForeignKey(
                        name: "FK_Invoices_EdiinvoicingTo_BillToParty",
                        column: x => x.BillToParty,
                        principalTable: "EdiinvoicingTo",
                        principalColumn: "BillToParty");
                    table.ForeignKey(
                        name: "FK_Invoices_InvoiceTypeEntity_InvoiceType",
                        column: x => x.InvoiceType,
                        principalTable: "InvoiceTypeEntity",
                        principalColumn: "InvoiceTypeId");
                    table.ForeignKey(
                        name: "FK_Invoices_User_InvoicedUser",
                        column: x => x.InvoicedUser,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "SapInvoices",
                columns: table => new
                {
                    AltInvoiceNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DistrChanel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Division = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extracted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExtractStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sapinvoice1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Changed = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SepInvoice", x => x.AltInvoiceNumber);
                    table.ForeignKey(
                        name: "FK_SapInvoices_Invoices_AltInvoiceNumber",
                        column: x => x.AltInvoiceNumber,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceNumber");
                });

            migrationBuilder.CreateTable(
                name: "InvoiceItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AltInvoiceNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Item = table.Column<short>(type: "smallint", nullable: true),
                    ServiceMaterial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BilledQty = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SalesUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BilledQtySku = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PricingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BasicPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Tax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OutputTax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Tariff = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Event = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ratio = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CancelledAltNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BilledQty1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SalesUnit1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContractId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConditionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayTaxClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayRatio = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PayBasicPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PayPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PayTax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Percentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceItems_SapInvoices_AltInvoiceNumber",
                        column: x => x.AltInvoiceNumber,
                        principalTable: "SapInvoices",
                        principalColumn: "AltInvoiceNumber");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankData_CompanyId",
                table: "BankData",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_AltInvoiceNumber",
                table: "InvoiceItems",
                column: "AltInvoiceNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_BillToParty",
                table: "Invoices",
                column: "BillToParty");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoicedUser",
                table: "Invoices",
                column: "InvoicedUser");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceType",
                table: "Invoices",
                column: "InvoiceType");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_SoldToParty",
                table: "Invoices",
                column: "SoldToParty");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceTypeEntity_VatClause",
                table: "InvoiceTypeEntity",
                column: "VatClause");

            migrationBuilder.CreateIndex(
                name: "IX_User_UserChanger",
                table: "User",
                column: "UserChanger");

            migrationBuilder.CreateIndex(
                name: "IX_User_UserCreator",
                table: "User",
                column: "UserCreator");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankData");

            migrationBuilder.DropTable(
                name: "BusinessSpaceArea");

            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.DropTable(
                name: "InvoiceEvents");

            migrationBuilder.DropTable(
                name: "InvoiceItems");

            migrationBuilder.DropTable(
                name: "InvoicePools");

            migrationBuilder.DropTable(
                name: "InvoiceStorages");

            migrationBuilder.DropTable(
                name: "TariffDescs");

            migrationBuilder.DropTable(
                name: "tariffs");

            migrationBuilder.DropTable(
                name: "VslVoyages");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "SapInvoices");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "CustomerMaster");

            migrationBuilder.DropTable(
                name: "EdiinvoicingTo");

            migrationBuilder.DropTable(
                name: "InvoiceTypeEntity");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Vatclause");
        }
    }
}
