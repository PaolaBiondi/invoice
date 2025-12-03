using eInvoice.domain.Adapters;
using eInvoice.domain.Models;
using eInvoice.domain.Repositories;
using eInvoice.domain.Services;
using eInvoice.infra.efc.Infra.Models;
using eInvoice.infra.mojEracun;
using eInvoice.infra.mojEracun.Dtos;
using eInvoice.infra.mojEracun.Scheme;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Oasis.Names.Specification.Ubl.Schema.Xsd.CommonAggregateComponents._2;
using Oasis.Names.Specification.Ubl.Schema.Xsd.CommonBasicComponents._2;
using Oasis.Names.Specification.Ubl.Schema.Xsd.Invoice._2;
using System.Text.RegularExpressions;

namespace eInvoice.Tests.InfraLayer
{
    public class InvoiceSenderTests : IAsyncLifetime
    {
        private IHost? _host;

        public async Task InitializeAsync()
        {
            _host = await EinvoiceTestFactory.CreateTestHost();
        }

        public async Task DisposeAsync()
        {
            if (_host is null) return;

            await _host.StopAsync();
            _host?.Dispose();
        }

        private IinvoiceSender InitializeInvoiceSender(IOptions<EinvoiceSettings>? options = null)
        {
            var logger = EinvoiceTestFactory.MockLogger<InvoiceSender>();
            var httpClient = _host?.Services.GetRequiredService<IHttpClientFactory>().CreateClient("MockedClient"); //_host?.GetTestClient();
            IOptions<EinvoiceSettings> configuration = options ?? _host?.Services.GetService<IOptions<EinvoiceSettings>>()!;
            var serializer = _host?.Services.GetService<IxmlInvoiceSerializer<InvoiceType>>();
            var invoiceRepository = _host?.Services.GetService<IInvoiceRepository>();

            return new InvoiceSender(logger, httpClient!, configuration, invoiceRepository!, serializer!);
        }

        [Fact]
        public void InvoiceSender_InitializeSuccess()
        {
            var sender = InitializeInvoiceSender();

            Assert.NotNull(sender);
        }

        [Fact]
        public void InvoiceSender_InitializeFail_UriConfigurationMissing()
        {
            var settings = new EinvoiceSettings
            {
                EinvoiceServiceUri = null!,
                ApiKey = "missing-key"
            };
            IOptions<EinvoiceSettings> configuration = Options.Create(settings);

            Assert.Throws<ArgumentNullException>(() => InitializeInvoiceSender(configuration));
        }

        [Obsolete]
        private void InvoiceType_Success()
        {
            var invoice = new InvoiceType
            {
                CustomizationId = new CustomizationIdType { Value = Globals.CUSTOMIZATION_ID },
                ProfileId = new ProfileIdType { Value = "P1" },
                Id = new IdType { Value = "8-P1-1" },
                IssueDate = new IssueDateType { Value = DateTime.Today },
                IssueTime = new IssueTimeType { _value = DateTime.Today },
                DueDate = new DueDateType { Value = DateTime.Today.AddDays(30) },
                InvoiceTypeCode = new InvoiceTypeCodeType { Value = Globals.INVOICE_TYPE_CODE },
                DocumentCurrencyCode = new DocumentCurrencyCodeType { Value = "EUR" },
                AccountingSupplierParty = new SupplierPartyType
                {
                    Party = new PartyType
                    {
                        EndpointId = new EndpointIdType
                        {
                            Value = "12345678901",
                            SchemeId = "9934"
                        },
                        PartyIdentification = {
                            new PartyIdentificationType
                                { Id = new IdType
                                    {
                                        Value = "12345678901"
                                        ,SchemeId = "9934"
                                    }
                                }
                        },
                        PostalAddress = new AddressType
                        {
                            StreetName = new StreetNameType { Value = "Ulica 1" },
                            CityName = new CityNameType { Value = "Zagreb" },
                            PostalZone = new PostalZoneType { Value = "10000" },
                            Country = new CountryType
                            {
                                IdentificationCode = new IdentificationCodeType { Value = "HR" }
                            }
                        },
                        PartyTaxScheme = {
                            new PartyTaxSchemeType
                                {
                                    CompanyId = new CompanyIdType{ Value = "12345678901" },
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
                                RegistrationName = new RegistrationNameType { Value = "Tvrtka A d.o.o." },
                                CompanyLegalForm = new CompanyLegalFormType{
                                    Value = "Tvrtka A d.o.o. osnovana na Trgovačkom sudu u Osijeku, temeljni kapital 20.000,00 EUR, direktor Ivan Perić, Odgovorna osoba: Marica Horvat",
                                }
                            }
                        },
                        Contact = new ContactType
                        {
                            Name = new NameType { Value = "IME PREZIME" },
                            Telephone = new TelephoneType { Value = "ii@mail.hr" }
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
                            Value = "11111111119",
                            SchemeId = "9934"
                        },
                        PostalAddress = new AddressType
                        {
                            StreetName = new StreetNameType { Value = "Ulica 2" },
                            CityName = new CityNameType { Value = "Zagreb" },
                            PostalZone = new PostalZoneType { Value = "10000" },
                            Country = new CountryType
                            {
                                IdentificationCode = new IdentificationCodeType { Value = "HR" }
                            }
                        },
                        PartyTaxScheme = {
                            new PartyTaxSchemeType
                                {
                                    CompanyId = new CompanyIdType{ Value = "HR11111111119" },
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
                        },
                        PartyIdentification = {
                            new PartyIdentificationType
                                { Id = new IdType
                                    {
                                        Value = "12345678901"
                                        ,SchemeId = "9934"
                                    }
                                }
                        }
                    }
                },
                LegalMonetaryTotal = new MonetaryTotalType
                {
                    LineExtensionAmount = new LineExtensionAmountType { Value = 100.00m, CurrencyId = "EUR" },
                    TaxExclusiveAmount = new TaxExclusiveAmountType { Value = 100.00m, CurrencyId = "EUR" },
                    TaxInclusiveAmount = new TaxInclusiveAmountType { Value = 100.00m, CurrencyId = "EUR" },
                    PayableAmount = new PayableAmountType { Value = 100.00m, CurrencyId = "EUR" }
                },
                InvoiceLine = {
                    new InvoiceLineType
                    {
                        Id = new IdType { Value = "1" },
                        InvoicedQuantity = new InvoicedQuantityType { Value = 1.00m, UnitCode = "H87" },
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
                                new TaxCategory
                                {
                                    Id = new IdType { Value = "Z" },
                                    Name = new NameType { Value = "HR:PDV0" },
                                    Percent = new PercentType { Value = 0 },
                                    TaxScheme = new TaxSchemeType
                                    {
                                        Id = new IdType { Value = "VAT" }
                                    }
                                }
                            }
                        },

                        Price = new PriceType
                        {
                            PriceAmount = new PriceAmountType { Value = 100.00m, CurrencyId = "EUR" },
                            BaseQuantity= new BaseQuantityType { Value = 1.00m, UnitCode= "H87" }

                        }
                    }
                }
            };

            invoice.PaymentMeans.Add(new PaymentMeansType
            {
                PaymentMeansCode = new PaymentMeansCodeType { Value = "30" },
                InstructionNote = { new InstructionNoteType { Value = "Neki opis plaćanja" } },
                PaymentId = { new PaymentIdType { Value = "HR00 123456" } },
                PayeeFinancialAccount = new FinancialAccountType
                {
                    Id = new IdType { Value = "HRXXXXXXXXXXXXXXXX" },
                    Name = new NameType { Value = "Račun za plaćanje" }
                }
            });
            invoice.TaxTotal.Add(new TaxTotalType
            {
                TaxAmount = new TaxAmountType { Value = 0.00m, CurrencyId = "EUR" },
                TaxSubtotal = {
                    new TaxSubtotalType
                    {
                        TaxableAmount = new TaxableAmountType { Value = 100.00m, CurrencyId = "EUR" },
                        TaxAmount = new TaxAmountType { Value = 0.00m, CurrencyId = "EUR" },
                        TaxCategory = new TaxCategoryType
                        {
                            Id = new IdType { Value = "Z" },
                            Percent = new PercentType { Value = 0 },
                            TaxScheme = new TaxSchemeType
                            {
                                Id = new IdType { Value = "VAT" }
                            }
                        }
                    }
                }
            });

            Assert.NotNull(invoice);
        }

        [Fact]
        public async Task Ping_Success()
        {
            var sender = InitializeInvoiceSender();
            
            var result = await sender.PingAsync();
            
            Assert.True(result);
        }

        [Fact]
        public async Task SendAsync_Fails_WrongAttributes()
        {
            var sender = InitializeInvoiceSender();
            string message = null!;
            string destination = null!;

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sender.SendAsync(message, destination));
        }

        [Fact]
        public async Task SendAsync_Fails_ResponseStatusNotOk()
        {
            var sender = InitializeInvoiceSender();
            string message = "null";
            string destination = "send";

            var apiResponse = await sender.SendAsync(message, destination);

            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.Errors.Any());
            Assert.NotNull(apiResponse.Errors.First().Message);
            Assert.True(apiResponse.Errors.First().Message.Contains("greška", StringComparison.CurrentCultureIgnoreCase));
        }

        [Theory]
        //[InlineData("951378")]
        //[InlineData("998603")]
        //[InlineData("998650")]
        //[InlineData("998800")]
        //[InlineData("994947")]
        //[InlineData("996090")]
        //[InlineData("993920")]
        //[InlineData("993919")]
        [InlineData("994590")]
        public async Task SendAsync_Success(string invoiceId)
        {
            
            var sender = InitializeInvoiceSender();
            var invoice = await ((IInvoiceSenderValue<InvoiceType>)sender).PrepareMessage(invoiceId);
            var serializer = _host?.Services.GetService<IxmlInvoiceSerializer<InvoiceType>>();
            var configuration = _host?.Services.GetService<IOptions<EinvoiceSettings>>()?.Value;

            var message = serializer?.Serialize(invoice);
           var destination = configuration?.EinvoiceServiceUri;

            var pattern = @"<cbc:ElectronicMail>[^<]+</cbc:ElectronicMail>";
            var replacement = "<cbc:ElectronicMail>ana.plazonic@agct.ictsi.com</cbc:ElectronicMail>";
            var result = Regex.Replace(message!, pattern, replacement);

            var apiResponse = await sender.SendAsync(result, "send");

            Assert.NotNull(apiResponse);
            Assert.True(!apiResponse.Errors.Any() && apiResponse.Value is not null);
        }

        [Theory]
        [InlineData("996789")]
        public async Task SendAsync_EmployeeEInvoiceShouldFail(string invoiceId)
        {
            var sender = InitializeInvoiceSender();
            var apiResponse = await sender.SendInvoiceAsync(invoiceId);

            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.Errors.Any() && apiResponse.Value is null);
            Assert.True(apiResponse.Errors.OfType<InvalidOperationException>().Any());
        }

        [Theory]
        //[InlineData("951378")]
        [InlineData("994947")]
        public async Task PrepareMessage_Success(string invoiceId)
        {
            InvoiceType? invoice = null;
            var sender = InitializeInvoiceSender();
            if (sender is IInvoiceSenderValue<InvoiceType> typedSender)
            {
                invoice = await typedSender.PrepareMessage(invoiceId);

            }
            Assert.NotNull(invoice);
            Assert.True(invoice.Id.Value.Equals(invoiceId, StringComparison.CurrentCultureIgnoreCase));
        }

        [Theory]
        [InlineData("80300395055", true)]
        [InlineData("79905898396", false)]
        public async Task CheckCompanyIdInAms_ForCompanyIdReturnsResponse(string companyId, bool response)
        {
            var sender = InitializeInvoiceSender();
            
            var result = await sender.CheckCompanyIdInAms(companyId);
            
            Assert.True(result == response);
        }

        [Fact]
        public async Task MarkPaid_Success()
        {
            var json = """
                {
                    "Username": "11818",
                    "Password": "q5GD46Xb",
                    "CompanyId": "80300395055",
                    "CompanyBu": "",
                    "SoftwareId": "Test-001",
                    "ElectronicId": 3077401,
                    "PaymentDate": "2025-08-19",
                    "PaymentAmount": 1250.75,
                    "PaymentMethod": "T"
                }
                """;

            var request = System.Text.Json.JsonSerializer.Deserialize<MarkPaidRequest>(json);


            var sender = InitializeInvoiceSender();
            var payload = new PaidInvoiceDto
            {
                CompanyId = "84596041174",
                PaymentDate = DateOnly.FromDateTime(DateTime.Parse("2025-06-30")),
                IssueDate = DateOnly.FromDateTime(DateTime.Parse("2025-06-15")),
                PaymentAmount = 60.00m,
                IsRegistered = true
            };

            var result = await sender.MarkPaid(payload);
            Assert.NotNull(result?.Value);
            Assert.False(result.Errors.Any());
            Assert.True(result.Value.FiscalizationDateTime.HasValue);
        }


        [Fact]
        public async Task MarkPaid_Failed()
        {
            var sender = InitializeInvoiceSender();
            PaidInvoiceDto payload = null!;

            var result = await sender.MarkPaid(payload);
            Assert.NotNull(result);
            Assert.True(result.Errors.Any());
            Assert.Null(result.Value);
        }
    }
}
