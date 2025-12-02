using eInvoice.domain.Models;
using eInvoice.domain.Repositories;
using eInvoice.domain.Services;
using eInvoice.infra.efc.Data;
using eInvoice.infra.efc.Repositories;
using eInvoice.infra.mojEracun;
using eInvoice.infra.mojEracun.Dtos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Oasis.Names.Specification.Ubl.Schema.Xsd.CommonAggregateComponents._2;
using Oasis.Names.Specification.Ubl.Schema.Xsd.CommonBasicComponents._2;
using Oasis.Names.Specification.Ubl.Schema.Xsd.CommonExtensionComponents._2;
using Oasis.Names.Specification.Ubl.Schema.Xsd.Invoice._2;
using Oasis.Names.Specification.Ubl.Schema.Xsd.SignatureAggregateComponents._2;
using System.Diagnostics.Eventing.Reader;
using System.Text.Json;
using System.Xml.Linq;

namespace eInvoice.Tests
{
    public enum TestInvoiceType
    {
        [FileName("eRacun-PDV0.xml")]
        PDV0,
        [FileName("eRacun-PDV13.xml")]
        PDV13,
        [FileName("eRacun-PDV25.xml")]
        PDV25
    }

    internal static class EinvoiceTestFactory
    {
        internal static InvoiceType StubCreatePDV0 = CreateInvoice("8-P1-1",
                                                                 taxPercent: 0,
                                                                 taxAmount: 0.00m,
                                                                 taxCategory: "Z",
                                                                 taxInclusiveAmount: 100.00m,
                                                                 payableAmount: 100.00m);
        internal static InvoiceType StubCreatePDV13 = CreateInvoice("6-P1-1", note: "Opis plaćanja");
        internal static InvoiceType StubCreatePDV25 = CreateInvoice("5-P1-1",
                                                              streetName: "VRTNI PUT 3",
                                                              partyName: "FINANCIJSKA AGENCIJA",
                                                              partyForm: null,
                                                              taxAmount: 25.00m,
                                                              taxPercent: 25,
                                                              taxCategory: "S",
                                                              taxInclusiveAmount: 125.00m,
                                                              payableAmount: 125.00m,
                                                              note: "Opis plaćanja"
            );

        internal static ILogger<T> MockLogger<T>() => NullLogger<T>.Instance; //preferred than new NullLogger<T>() as it will use singleton

        internal static async Task<IHost> CreateTestHost()
        {
            var hostBuilder = new HostBuilder().ConfigureWebHost(webCfg =>
            {
                webCfg.UseTestServer()
                .ConfigureAppConfiguration(config =>
                {
                    config.AddJsonFile("appsettings.Development.json", optional: false);
                })
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                    services.AddHttpClient("MockedClient")
                            .ConfigurePrimaryHttpMessageHandler<StubHttpMessageHandler>();

                    services.AddOptions<EinvoiceSettings>()
                            .BindConfiguration(nameof(EinvoiceSettings))
                            .ValidateDataAnnotations();

                    services.AddTransient<StubHttpMessageHandler>();
                    services.AddTransient<IxmlInvoiceSerializer<InvoiceType>, XmlInvoiceSerializer>();

                    services.AddTransient<IInvoiceRepository, InvoiceRepository>();
                    services.AddTransient<IPaidInvoiceRepository, PaidInvoicesRepositroy>();
                    services.AddTransient<IRejectedInvoiceRepository, RejectedInvoiceRepository>();

                    services.AddDbContext<BillingContext>((serviceProvider, options) =>
                    {
                        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                        var connectionString = configuration.GetConnectionString("BillingConnection");
                        options.UseSqlServer(connectionString);
                    });
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("ping", async context =>
                        {
                            context.Response.ContentType = "application/json";
                            var response = new
                            {
                                Status = "ok",
                                Message = $"Service is up @{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}"
                            };
                            await context.Response.WriteAsJsonAsync(response);
                        });
                    });
                });
            });

            return await hostBuilder.StartAsync();
        }

        internal static XDocument? ExpectedDocument(TestInvoiceType invoiceType)
        {
            var xmlName = GetFileName(invoiceType);

            var refXml = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "assets", xmlName));
            var expectedXml = XDocument.Parse(refXml);

            return expectedXml;
        }

        internal static InvoiceType CreateInvoice(TestInvoiceType invoiceType)
        {
            var invoice = invoiceType switch
            {
                TestInvoiceType.PDV0 => StubCreatePDV0,
                TestInvoiceType.PDV13 => StubCreatePDV13,
                TestInvoiceType.PDV25 => StubCreatePDV25,
                _ => throw new NotImplementedException()
            };

            return invoice;
        }

        private static InvoiceType CreateInvoice(string invoiceId,
                                               string companyId = "12345678901",
                                               string partyName = "Tvrtka A d.o.o.",
                                               string? partyForm = "Tvrtka A d.o.o. osnovana na Trgovačkom sudu u Osijeku, temeljni kapital 20.000,00 EUR, direktor Ivan Perić, Odgovorna osoba: Marica Horvat",
                                               string streetName = "Ulica 1",
                                               string customerId = "11111111119",
                                               decimal taxPercent = 13m,
                                               decimal taxAmount = 13.00m,
                                               string taxCategory = "S",
                                               decimal lineExtensionAmount = 100.00m,
                                               decimal taxExclusiveAmount = 100.00m,
                                               decimal taxInclusiveAmount = 113.00m,
                                               decimal payableAmount = 113.00m,
                                               string note = "Neki opis plaćanja")
        {
            var timeStamp = new DateTime(2025, 5, 1);
            var schemeId = "9934";
            var invoice = new InvoiceType
            {
                UblExtensions = {new UblExtensionType
                {
                    ExtensionContent = new ExtensionContentType
                    {
                        UblDocumentSignatures = { new SignatureInformationType { Any = null} }
                    }
                } },

                CustomizationId = new CustomizationIdType { Value = Globals.CUSTOMIZATION_ID },
                ProfileId = new ProfileIdType { Value = "P1" },
                Id = new IdType { Value = invoiceId },
                IssueDate = new IssueDateType { Value = timeStamp },
                IssueTime = new IssueTimeType { _value = timeStamp.AddHours(12) },
                DueDate = new DueDateType { Value = timeStamp.AddDays(30) },
                InvoiceTypeCode = new InvoiceTypeCodeType { Value = Globals.INVOICE_TYPE_CODE },
                DocumentCurrencyCode = new DocumentCurrencyCodeType { Value = "EUR" },
                AccountingSupplierParty = new SupplierPartyType
                {
                    Party = new PartyType
                    {
                        EndpointId = new EndpointIdType
                        {
                            Value = companyId,
                            SchemeId = schemeId
                        },
                        PartyIdentification = {
                            new PartyIdentificationType
                                { Id = new IdType
                                    {
                                        Value = $"{schemeId}:{companyId}"
                                    }
                                }
                        },
                        PostalAddress = new AddressType
                        {
                            StreetName = new StreetNameType { Value = streetName },
                            CityName = new CityNameType { Value = "ZAGREB" },
                            PostalZone = new PostalZoneType { Value = "10000" },
                            Country = new CountryType
                            {
                                IdentificationCode = new IdentificationCodeType { Value = "HR" }
                            }
                        },
                        PartyTaxScheme = {
                            new PartyTaxSchemeType
                                {
                                    CompanyId = new CompanyIdType{ Value = $"HR{companyId}" },
                                    TaxScheme = new TaxSchemeType
                                    {
                                        Id = new IdType { Value = "VAT" }
                                    }
                                }
                        },
                        PartyLegalEntity =
                        {
                            new PartyLegalEntityType
                            {
                                RegistrationName = new RegistrationNameType { Value = partyName },
                                CompanyLegalForm = string.IsNullOrWhiteSpace(partyForm) ? null
                                                                                        : new CompanyLegalFormType{Value = partyForm}
                            }
                        },
                        Contact = new ContactType
                        {
                            Name = new NameType { Value = "IME PREZIME" },
                            ElectronicMail = new ElectronicMailType { Value = "ii@mail.hr" }
                        }
                    },
                    SellerContact = new ContactType
                    {
                        Id = new IdType { Value = "51634872748" },
                        Name = new NameType { Value = "Operater1" }
                    }
                },
                AccountingCustomerParty = new CustomerPartyType
                {
                    Party = new PartyType
                    {
                        EndpointId = new EndpointIdType
                        {
                            Value = customerId,
                            SchemeId = schemeId
                        },
                        PostalAddress = new AddressType
                        {
                            StreetName = new StreetNameType { Value = "Ulica 2" },
                            CityName = new CityNameType { Value = "RIJEKA" },
                            PostalZone = new PostalZoneType { Value = "51000" },
                            Country = new CountryType
                            {
                                IdentificationCode = new IdentificationCodeType { Value = "HR" }
                            }
                        },
                        PartyTaxScheme = {
                            new PartyTaxSchemeType
                                {
                                    CompanyId = new CompanyIdType{ Value = $"HR{customerId}" },
                                    TaxScheme = new TaxSchemeType
                                    {
                                        Id = new IdType { Value = "VAT" }
                                    }
                                }
                        },
                        PartyLegalEntity =
                        {
                            new PartyLegalEntityType
                            {
                                RegistrationName = new RegistrationNameType { Value = "Tvrtka B d.o.o." }
                            }
                        }
                    }
                },
                LegalMonetaryTotal = new MonetaryTotalType
                {
                    LineExtensionAmount = new LineExtensionAmountType { Value = lineExtensionAmount, CurrencyId = "EUR" },
                    TaxExclusiveAmount = new TaxExclusiveAmountType { Value = taxExclusiveAmount, CurrencyId = "EUR" },
                    TaxInclusiveAmount = new TaxInclusiveAmountType { Value = taxInclusiveAmount, CurrencyId = "EUR" },
                    PayableAmount = new PayableAmountType { Value = payableAmount, CurrencyId = "EUR" }
                },
                InvoiceLine = {
                    new InvoiceLineType
                    {
                        Id = new IdType { Value = "1" },
                        InvoicedQuantity = new InvoicedQuantityType { Value = 1.000m, UnitCode = "H87" },
                        LineExtensionAmount = new LineExtensionAmountType { Value = 100.00m, CurrencyId = "EUR" },
                        Item = new ItemType
                        {
                            Name = new NameType { Value = "Proizvod" },
                            CommodityClassification = {
                                new CommodityClassificationType
                                {
                                    ItemClassificationCode = new ItemClassificationCodeType { Value = "62.90.90", ListId = Globals.LIST_ID }
                                }
                            },
                            ClassifiedTaxCategory = {
                                new TaxCategoryType
                                {
                                    Id = new IdType { Value = taxCategory },
                                    Name = new NameType { Value = $"HR:PDV{taxPercent}" },
                                    Percent = new PercentType { Value = taxPercent },
                                    TaxScheme = new TaxSchemeType
                                    {
                                        Id = new IdType { Value = "VAT" }
                                    }
                                }
                            }
                        },

                        Price = new PriceType
                        {
                            PriceAmount = new PriceAmountType { Value = 100.000000m, CurrencyId = "EUR" },
                            BaseQuantity= new BaseQuantityType { Value = 1.000m, UnitCode= "H87" }

                        }
                    }
                }
            };

            invoice.PaymentMeans.Add(new PaymentMeansType
            {
                PaymentMeansCode = new PaymentMeansCodeType { Value = "30" },
                InstructionNote = { new InstructionNoteType { Value = note } },
                PaymentId = { new PaymentIdType { Value = "HR00 123456" } },
                PayeeFinancialAccount = new FinancialAccountType
                {
                    Id = new IdType { Value = "HRXXXXXXXXXXXXXXXX" }
                }
            });
            invoice.TaxTotal.Add(new TaxTotalType
            {
                TaxAmount = new TaxAmountType { Value = taxAmount, CurrencyId = "EUR" },
                TaxSubtotal = {
                    new TaxSubtotalType
                    {
                        TaxableAmount = new TaxableAmountType { Value = 100.00m, CurrencyId = "EUR" },
                        TaxAmount = new TaxAmountType { Value = taxAmount, CurrencyId = "EUR" },
                        TaxCategory = new TaxCategoryType
                        {
                            Id = new IdType { Value = taxCategory },
                            Percent = new PercentType { Value = taxPercent },
                            TaxScheme = new TaxSchemeType
                            {
                                Id = new IdType { Value = "VAT" }
                            }
                        }
                    }
                }
            });

            return invoice;
        }

        private static string GetFileName(TestInvoiceType invoiceType)
        {
            var member = typeof(TestInvoiceType).GetMember(invoiceType.ToString()).First();
            var att = (FileNameAttribute)Attribute.GetCustomAttribute(member, typeof(FileNameAttribute))!;
            return att?.FileName ?? string.Empty;
        }

        internal static MarkPaidResponse GenerateMarkPaidFailedResponse()
        {
            var json = """
                {
                    "message": "Pogreška prilikom obrade zahtjeva na strani porezne uprave. Poruka: Već postoji zabilježen eRačun s istim identifikatorom.",
                    "traceId": "e8169e93-bac7-4ec6-9247-948aa226358f",
                    "taxAdministrationSpecifics": {
                        "code": "S008",
                        "description": "Već postoji zabilježen eRačun s istim identifikatorom"
                    }
                }
                """;

            MarkPaidResponse response = JsonSerializer.Deserialize<MarkPaidResponse>(json) ?? new MarkPaidResponse();
            return response;
        }

        internal static PaidInvoiceDto GeneratePaidInvoiceDto(string invoiceId = "", bool isRegistered = false, bool hasMessage = false)
        {
            return new PaidInvoiceDto
            {
                InvoiceNumber = invoiceId,
                CompanyId = "12345678901",
                Message = hasMessage ? "TW9jayBYTUwgY29udGVudCBmb3IgZmlzY2FsaXphdGlvbiByZXF1ZXN0": null,
                FiscalizationDateTime = hasMessage ? DateTimeOffset.Now: null
            };
        }

        internal static RejectedInvoiceDto GenerateRejectedInvoiceDto(string invoiceId = "", bool isRegistered = false, bool hasMessage = false)
        {
            return new RejectedInvoiceDto
            {
                InvoiceNumber = invoiceId,
                CompanyId = "12345678901",
                Message = hasMessage ? "TW9jayBYTUwgY29udGVudCBmb3IgZmlzY2FsaXphdGlvbiByZXF1ZXN0" : null,
                FiscalizationDateTime = hasMessage ? DateTimeOffset.Now : null,
                IsRegistered = isRegistered,
            };
        }
    }
}
