using eInvoice.domain.Common;
using eInvoice.domain.Models;
using eInvoice.domain.Repositories;
using eInvoice.domain.Services;
using eInvoice.infra.mojEracun.Dtos;
using eInvoice.infra.mojEracun.Scheme;
using Mfin.Gov.Hr.Schema.Xsd.HrExtensionAggregateComponents._1;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Oasis.Names.Specification.Ubl.Schema.Xsd.CommonAggregateComponents._2;
using Oasis.Names.Specification.Ubl.Schema.Xsd.CommonBasicComponents._2;
using Oasis.Names.Specification.Ubl.Schema.Xsd.CommonExtensionComponents._2;
using Oasis.Names.Specification.Ubl.Schema.Xsd.Invoice._2;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace eInvoice.infra.mojEracun;

internal class InvoiceSender : IinvoiceSender, IInvoiceSenderValue<InvoiceType>
{
    private readonly ILogger<InvoiceSender> logger;
    private HttpClient _httpClient;
    private readonly EinvoiceSettings configuration;
    private readonly IInvoiceRepository invoiceRepository;
    private readonly IxmlInvoiceSerializer<InvoiceType> serializer;

    public InvoiceSender(ILogger<InvoiceSender> logger,
                         HttpClient httpClient,
                         IOptions<EinvoiceSettings> configuration,
                         IInvoiceRepository invoiceRepository,
                         IxmlInvoiceSerializer<InvoiceType> serializer)
    {
        this.logger = logger;
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.configuration = configuration.Value;
        this.invoiceRepository = invoiceRepository;
        this.serializer = serializer;
        if (_httpClient.BaseAddress is null)
            _httpClient.BaseAddress = new Uri(configuration.Value.EinvoiceServiceUri);
        _httpClient.DefaultRequestHeaders.Add("X-API-KEY", configuration.Value.ApiKey);
    }

    public async Task<AdapterResponse<UpdateElectronicInvoice>> SendAsync(string message, string destination, CancellationToken cancellationToken = default)
    {
        logger.LogTrace($"Start {nameof(SendAsync)}");
        InvoiceUploadResponse? invoiceUploadResponse = null;

        if (string.IsNullOrWhiteSpace(message)
            || string.IsNullOrWhiteSpace(destination))
        {
            throw new ArgumentNullException($"{nameof(message)} or {nameof(destination)} cannot be null or empty.");
        }

        try
        {
            var payload = BaseRequestDto.Build<InvoiceUploadRequest>(configuration);

            payload.CompanyBu = configuration.CompanyBu ?? string.Empty;
            payload.File = message;

            logger.LogTrace("Sending invoice {@payload} to {Destination}", payload, destination);

            var response = await _httpClient.PostAsJsonAsync(destination, payload, cancellationToken);
            var responseMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogTrace("Response message {statusCode} received: {response}", response.StatusCode, responseMessage);

            invoiceUploadResponse = (await response.Content.ReadFromJsonAsync<InvoiceUploadResponse>(cancellationToken))
                                    ?? throw new InvalidOperationException("Unexpected response received from eInvoice");

            if (invoiceUploadResponse?.Errors is not null)
            {
                var err = new InvalidOperationException(string.Join(',', invoiceUploadResponse.Errors.FirstOrDefault().Value));
                return AdapterResponse<UpdateElectronicInvoice>.Failure(err);
            }

            var updateElectronicInvoice = new UpdateElectronicInvoice
            {
                InvoiceNumber = invoiceUploadResponse!.DocumentNr!,
                Ogds = Encoding.UTF8.GetBytes(message),
                ElectronicId = invoiceUploadResponse.ElectronicId,
                Created = invoiceUploadResponse!.Created,
                StatusId = invoiceUploadResponse.StatusId,
                Sent = invoiceUploadResponse!.Sent
            };

            return AdapterResponse<UpdateElectronicInvoice>.Success(updateElectronicInvoice);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return AdapterResponse<UpdateElectronicInvoice>.Failure(ex);
        }
        finally
        {
            logger.LogTrace($"End {nameof(SendAsync)}");
        }
    }

    private string PreparePayload(InvoiceType invoice)
    {
        logger.LogTrace($"Start {nameof(PreparePayload)}");
        if (invoice == null)
        {
            throw new ArgumentNullException(nameof(invoice), "Invoice cannot be null.");
        }

        return serializer.Serialize(invoice);
    }

    public async Task<InvoiceType> PrepareMessage(string invoiceId)
    {
        try
        {
            var source = await invoiceRepository.GetInvoiceAsync(invoiceId);

            var ublInvoice = new InvoiceType
            {
                CustomizationId = new CustomizationIdType { Value = Globals.CUSTOMIZATION_ID },
                ProfileId = new ProfileIdType { Value = source.ProfileId },
                Id = new IdType { Value = invoiceId },
                IssueDate = new IssueDateType { Value = source.IssueDateTime },
                IssueTime = new IssueTimeType { _value = source.IssueDateTime },
                DueDate = new DueDateType { Value = source.IssueDateTime },
                InvoiceTypeCode = new InvoiceTypeCodeType { Value = (source.ProfileId!.Equals(nameof(BusinessProcessProfileQualifier.P10), StringComparison.InvariantCultureIgnoreCase) ? (int)InvoiceTypeCodeQualifier.Corrective : (int)InvoiceTypeCodeQualifier.Commercial).ToString() },
                DocumentCurrencyCode = new DocumentCurrencyCodeType { Value = source.Currency },
                CopyIndicator = new CopyIndicatorType { Value = source.CopyIndicator },
                AccountingSupplierParty = new SupplierPartyType
                {
                    Party = new PartyType
                    {
                        PartyName = {
                        new PartyNameType
                        {
                            Name = new NameType { Value = source.Supplier.Name }
                        }
                    },
                        EndpointId = new EndpointIdType
                        {
                            Value = source.Supplier.TaxNumber,
                            SchemeId = Globals.SCHEME_ID
                        },
                        PartyIdentification = {
                        new PartyIdentificationType
                        {
                            Id = new IdType
                            {
                                Value =  $"{Globals.SCHEME_ID}:{source.Supplier.TaxNumber}"
                            }
                        }
                    },
                        PostalAddress = new AddressType
                        {
                            StreetName = new StreetNameType { Value = source.Supplier.Street },
                            BuildingNumber = new BuildingNumberType { Value = source.Supplier.BuildingNumber },
                            CityName = new CityNameType { Value = source.Supplier.City },
                            PostalZone = new PostalZoneType { Value = source.Supplier.PostalCode },
                            AddressLine = { new AddressLineType { Line = new LineType { Value = source.Supplier.AddressLine } } },
                            Country = new CountryType
                            {
                                IdentificationCode = new IdentificationCodeType { Value = source.Supplier.Country }
                            }
                        },
                        PartyTaxScheme = {
                        new PartyTaxSchemeType
                            {
                                CompanyId = new CompanyIdType{ Value = $"HR{source.Supplier.TaxNumber}" },
                                TaxScheme = new TaxSchemeType
                                {
                                    Id = new IdType { Value = Globals.TAX_SCHEME_ID }
                                }
                            }
                    },
                        PartyLegalEntity = {
                        new PartyLegalEntityType
                        {
                            RegistrationName = new RegistrationNameType { Value = source.Supplier.Name },
                            CompanyId = new CompanyIdType { Value = source.Supplier.TaxNumber }
                        }
                    },
                        Contact = new ContactType
                        {
                            Name = new NameType { Value = source.Supplier.ContactName },
                            ElectronicMail = new ElectronicMailType { Value = source.Supplier.Email }
                        }
                    },
                    SellerContact = new ContactType
                    {
                        Id = new IdType { Value = source.Supplier.TaxNumber }, // TODO OIB of operator or OIB of company? in the POSTMAN example it is OIB operatera, in the excel .. Polje OIB-a operatera se koristi samo ako je to potrebno.. 
                        Name = new NameType { Value = source.Supplier.ContactName }
                    }
                },
                AccountingCustomerParty = new CustomerPartyType
                {
                    Party = new PartyType
                    {
                        PartyName = {
                        new PartyNameType
                        {
                            Name = new NameType { Value = source.Customer.Name }
                        }
                    },
                        EndpointId = new EndpointIdType
                        {
                            Value = source.Customer.TaxNumber,
                            SchemeId = Globals.SCHEME_ID
                        },
                        PostalAddress = new AddressType
                        {
                            StreetName = new StreetNameType { Value = source.Customer.Street },
                            CityName = new CityNameType { Value = source.Customer.City },
                            PostalZone = new PostalZoneType { Value = source.Customer.PostalCode },
                            BuildingNumber = new() { Value = source.Customer.BuildingNumber },
                            Country = new CountryType
                            {
                                IdentificationCode = new IdentificationCodeType { Value = source.Customer.Country }
                            }
                        },
                        PartyTaxScheme = {
                            new PartyTaxSchemeType
                                {
                                    CompanyId = new CompanyIdType{ Value = $"HR{source.Customer.TaxNumber}" },
                                    TaxScheme = new TaxSchemeType
                                    {
                                        Id = new IdType { Value = Globals.TAX_SCHEME_ID }
                                    }
                                }
                        },
                        PartyLegalEntity =
                        {
                            new PartyLegalEntityType
                            {
                                RegistrationName = new RegistrationNameType { Value = source.Customer.Name },
                                CompanyId = new CompanyIdType { Value = source.Customer.TaxNumber }
                            }
                        },
                        Contact = new ContactType
                        {
                            ElectronicMail = new ElectronicMailType { Value = source.Customer.Email }
                        }
                    }
                },
                PaymentMeans = {
                new PaymentMeansType
                {
                    PaymentMeansCode = new PaymentMeansCodeType { Value = Globals.PAYMENT_MEANS_CODE },
                    PaymentDueDate = new PaymentDueDateType { Value = source.BaselineDateTime },
                    PaymentChannelCode = new PaymentChannelCodeType { Value = Globals.PAYMENT_CHANNEL_CODE },
                    InstructionId = new InstructionIdType { Value = invoiceId },
                    InstructionNote = {
                        new InstructionNoteType { Value = Globals.INSTRUCTION_NOTE }
                    },
                    PaymentId = {
                        new PaymentIdType { Value = Globals.PAYMENT_ID }
                    },
                    PayeeFinancialAccount = new FinancialAccountType
                    {
                        Id = new IdType { Value = source.BankData.Iban },
                        CurrencyCode = new CurrencyCodeType { Value =  source.Currency},
                    }
                }
            },
                LegalMonetaryTotal = new MonetaryTotalType
                {
                    LineExtensionAmount = new LineExtensionAmountType { Value = Math.Round(source.MonetaryTotal.LineExtensionAmount, 2), CurrencyId = source.Currency },
                    TaxExclusiveAmount = new TaxExclusiveAmountType { Value = Math.Round(source.MonetaryTotal.TaxExclusiveAmount, 2), CurrencyId = source.Currency },
                    TaxInclusiveAmount = new TaxInclusiveAmountType { Value = Math.Round(source.MonetaryTotal.TaxInclusiveAmount, 2), CurrencyId = source.Currency },
                    PayableAmount = new PayableAmountType { Value = Math.Round(source.MonetaryTotal.PayableAmount, 2), CurrencyId = source.Currency }
                }
            };

            AddBillingReference(ublInvoice, source);
            AddTaxTotal(ublInvoice, source.TaxTotalDto, source.Currency);
            AddHrTaxExtension(ublInvoice, source);
            AddInvoiceLines(ublInvoice, source);
            await AddAttachments(ublInvoice, source);

            var payload = PreparePayload(ublInvoice);

            return ublInvoice;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error preparing invoice message for InvoiceId: {InvoiceId}", invoiceId);
            throw;
        }
    }

    private void AddBillingReference(InvoiceType ublInvoice, InvoiceDto source)
    {
        if (source is null || source.BillingReference is null)
            return;

        ublInvoice.BillingReference.Add(new BillingReferenceType
        {
            InvoiceDocumentReference = new DocumentReferenceType
            {
                Id = new IdType { Value = source.BillingReference.Id },
                IssueDate = new IssueDateType { Value = source.BillingReference.IssueDate }
            }
        });
    }

    private void AddTaxTotal(InvoiceType ublInvoice, TaxTotalDto taxTotalDto, string currencyId)
    {
        if (taxTotalDto is null)
            return;

        var taxTotal = new TaxTotalType
        {
            TaxAmount = new TaxAmountType { Value = Math.Round(taxTotalDto.TaxAmount, 2), CurrencyId = currencyId },
        };

        foreach (var item in CreateTaxSubtotals(taxTotalDto.TaxSubtotals, currencyId))
        {
            taxTotal.TaxSubtotal.Add(item);
        }

        ublInvoice.TaxTotal.Add(taxTotal);
    }

    private void AddHrTaxExtension(InvoiceType ublInvoice, InvoiceDto source)
    {
        if (source.TaxTotalDto.TaxSubtotals.Any(i => i.TaxCategory.IsHrBr26()))
        {
            foreach (var item in source.TaxTotalDto.TaxSubtotals)
            {
                var taxSubtotal = new HrTaxSubtotalType
                {
                    TaxableAmount = new TaxableAmountType { Value = Math.Round(item.TaxableAmount, 2), CurrencyId = source.Currency },
                    TaxAmount = new TaxAmountType { Value = Math.Round(item.TaxAmount, 2), CurrencyId = source.Currency },
                    HrTaxCategory = new HrTaxCategoryType
                    {
                        Id = new IdType { Value = item.TaxCategory.IsHrBr26() ? "0" : item.TaxCategory.Id },
                        Name = new NameType { Value = item.TaxCategory.Name },
                        Percent = new PercentType { Value = item.TaxCategory.Percent },
                        TaxExemptionReason = { new TaxExemptionReasonType { Value = item.TaxCategory.ExemptionReason } },
                        HrTaxScheme = new HrTaxSchemeType
                        {
                            Id = new IdType { Value = Globals.TAX_SCHEME_ID }
                        }
                    }
                };

                ublInvoice.UblExtensions.Add(new UblExtensionType
                {
                    ExtensionContent = new ExtensionContentType
                    {
                        Hrfisk20Data = new Hrfisk20DataType()
                    }
                });

                var hrTaxTotal = ((ublInvoice.UblExtensions.First().ExtensionContent ??= new ExtensionContentType()).Hrfisk20Data ??= new Hrfisk20DataType()).HrTaxTotal ??= new HrTaxTotalType();
                hrTaxTotal.HrTaxSubtotal.Add(taxSubtotal);
            }
        }


        if (!ublInvoice.UblExtensions.Any(e => e.ExtensionContent?.Hrfisk20Data != null))
            return;

        var hrTaxNode = ublInvoice.UblExtensions.First(e => e.ExtensionContent?.Hrfisk20Data != null).ExtensionContent.Hrfisk20Data;

        // tax amount is sum of all Subtotal's taxamount where taxCategory is not 0, i.e. where item is not exempted from tax
        hrTaxNode.HrTaxTotal.TaxAmount = new TaxAmountType
        {
            Value = hrTaxNode.HrTaxTotal.HrTaxSubtotal.Where(ts => !ts.HrTaxCategory.Id.Value.Equals("0")).Sum(ts => Math.Round(ts.TaxAmount.Value, 2)),
            CurrencyId = hrTaxNode.HrTaxTotal.HrTaxSubtotal.First().TaxAmount.CurrencyId
        };

        hrTaxNode.HrLegalMonetaryTotal ??= new HrMonetaryTotalType();

        decimal outOfScopeAmount = 0m;
        decimal taxExclusiveAmount = 0m;

        foreach (var item in hrTaxNode.HrTaxTotal.HrTaxSubtotal.Where(ts => ts.HrTaxCategory.Id.Value.Equals("0")))
        {
            outOfScopeAmount += item.TaxAmount.Value;
            taxExclusiveAmount += item.TaxableAmount.Value;
        }
        hrTaxNode.HrLegalMonetaryTotal.OutOfScopeOfVatAmount = new OutOfScopeOfVatAmountType
        {
            Value = Math.Round(outOfScopeAmount, 2),
            CurrencyId = ublInvoice.LegalMonetaryTotal.TaxExclusiveAmount.CurrencyId
        };
        hrTaxNode.HrLegalMonetaryTotal.TaxExclusiveAmount = new TaxExclusiveAmountType
        {
            Value = Math.Round(taxExclusiveAmount, 2),
            CurrencyId = ublInvoice.LegalMonetaryTotal.TaxExclusiveAmount.CurrencyId
        };
    }

    private void AddInvoiceLines(InvoiceType ublInvoice, InvoiceDto source)
    {
        foreach (var item in source.InvoiceLines)
        {
            TaxExemptionReasonType? taxExemption = string.IsNullOrWhiteSpace(item.TaxCategory.ExemptionReason) ? null : new TaxExemptionReasonType { Value = item.TaxCategory.ExemptionReason };

            ublInvoice.InvoiceLine.Add(new InvoiceLineType
            {
                Id = new IdType { Value = item.ItemNumber.ToString() },
                InvoicedQuantity = new InvoicedQuantityType
                {
                    Value = Math.Round(item.Quantity, 3),
                    UnitCode = Globals.UNIT_CODE
                },
                LineExtensionAmount = new LineExtensionAmountType { Value = Math.Round(item.LineExtensionAmount, 3), CurrencyId = source.Currency },
                Item = new ItemType
                {
                    Name = new NameType { Value = item.TariffDescription },
                    CommodityClassification = {
                                new CommodityClassificationType
                                {
                                    ItemClassificationCode = new ItemClassificationCodeType { Value = item.CommodityClassification.ClassificationCode, ListId = Globals.LIST_ID }
                                }
                    },
                    ClassifiedTaxCategory = {
                                new TaxCategoryType
                                {
                                    Id = new IdType { Value = item.TaxCategory.Id },
                                    Name = new NameType { Value = item.TaxCategory.Name},
                                    Percent = new PercentType { Value = item.TaxCategory.Percent },
                                    TaxExemptionReason = { taxExemption},
                                    TaxScheme = new TaxSchemeType
                                    {
                                        Id = new IdType { Value = Globals.TAX_SCHEME_ID }
                                    }
                                }
                    },
                    SellersItemIdentification = new ItemIdentificationType
                    {
                        Id = new IdType { Value = item.ServiceMaterial }
                    },
                },

                Price = new PriceType
                {
                    PriceAmount = new PriceAmountType { Value = Math.Abs((Math.Round((item.PayBasicPrice ?? 0m), 6))), CurrencyId = source.Currency },
                    BaseQuantity = new BaseQuantityType { Value = 1, UnitCode = Globals.UNIT_CODE }

                }
            });
        }
    }

    private async Task AddAttachments(InvoiceType ublInvoice, InvoiceDto source)
    {
        var att = await invoiceRepository.GetInvoiceAttachmentsAsync(source.Id);
        if (att != null && att.StrBase64 is not null)
        {
            foreach (var doc in att.StrBase64)
            {
                ublInvoice.AdditionalDocumentReference.Add(new DocumentReferenceType
                {
                    Id = new IdType { Value = ublInvoice.Id.Value },
                    IssueDate = new IssueDateType { Value = ublInvoice.IssueDate.Value },
                    IssueTime = new() { Value = ublInvoice.IssueTime.Value },
                    Attachment = new AttachmentType
                    {
                        EmbeddedDocumentBinaryObject = new EmbeddedDocumentBinaryObjectType
                        {
                            EncodingCode = "base64",
                            MimeCode = doc.memeCode.GetMimeType(),
                            Filename = doc.fileName,
                            Value = Convert.FromBase64String(doc.content)
                        }
                    },
                    IssuerParty = new PartyType
                    {
                        PartyName = { new PartyNameType
                        {
                            Name = new NameType { Value = source.Supplier.Name }
                        }},
                        PostalAddress = new AddressType
                        {
                            AddressLine = { new() { Line = new() { Value = source.Supplier.AddressLine } } },
                            Country = new CountryType
                            {
                                IdentificationCode = new IdentificationCodeType { Value = Globals.COUNTRY }
                            }
                        }
                        //,
                        //PartyLegalEntity = { new PartyLegalEntityType
                        //{
                        //    RegistrationName = new RegistrationNameType { Value = source.Supplier.Name }
                        //    //CompanyId = new CompanyIdType { Value = $"HR{source.Supplier.TaxNumber}" }
                        //}}
                    }
                });
            }
        }
    }
    private IEnumerable<TaxSubtotalType> CreateTaxSubtotals(IList<TaxSubtotalDto> taxSubtotalDtos, string currencyId)
    {
        foreach (var dto in taxSubtotalDtos)
        {
            TaxExemptionReasonType? taxExemption = string.IsNullOrWhiteSpace(dto.TaxCategory.ExemptionReason) ? null : new TaxExemptionReasonType { Value = dto.TaxCategory.ExemptionReason };
            yield return new TaxSubtotalType
            {
                TaxableAmount = new TaxableAmountType { Value = Math.Round(dto.TaxableAmount, 2), CurrencyId = currencyId },
                TaxAmount = new TaxAmountType { Value = Math.Round(dto.TaxAmount, 2), CurrencyId = currencyId },
                TaxCategory = new TaxCategoryType
                {
                    Id = new IdType { Value = dto.TaxCategory.Id },
                    Percent = new PercentType { Value = dto.TaxCategory.Percent },
                    TaxExemptionReason = { taxExemption },
                    TaxScheme = new TaxSchemeType
                    {
                        Id = new IdType { Value = Globals.TAX_SCHEME_ID },
                        TaxTypeCode = new TaxTypeCodeType { Value = dto.TaxCategory.TaxTypeCode }
                    }
                }
            };
        }
    }

    public async Task<bool> PingAsync()
    {
        logger.LogTrace($"Start {nameof(PingAsync)}");
        try
        {
            var response = await _httpClient.GetAsync(configuration.Ping);
            logger.LogTrace("{Response} received", response);
            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content.ReadFromJsonAsync<PingResponse>();

            if (apiResponse is null || apiResponse.Status.Equals("error", StringComparison.CurrentCultureIgnoreCase))
                return false;

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return false;
        }
        finally
        {
            logger.LogTrace($"End {nameof(PingAsync)}");
        }
    }

    public async Task<AdapterResponse<UpdateElectronicInvoice>> SendInvoiceAsync(string invoiceId, bool maskContactinfo = true, CancellationToken cancellationToken = default)
    {
        var invoice = await PrepareMessage(invoiceId);
        var message = serializer.Serialize(invoice);
        if (maskContactinfo)
        {
            logger.LogInformation("Masking contact info for invoice {InvoiceId}", invoiceId);
            var pattern = @"<cbc:ElectronicMail>[^<]+</cbc:ElectronicMail>";
            var replacement = $"<cbc:ElectronicMail>{configuration.MaskedEmail}</cbc:ElectronicMail>";
            message = Regex.Replace(message, pattern, replacement);
        }

        var response = await SendAsync(message, "send");

        return response;
    }

    /// <summary>
    /// checks if a given companyId is registered in the AMS system
    /// </summary>
    /// <returns></returns>
    public async Task<bool> CheckCompanyIdInAms(string companyId, CancellationToken cancellationToken = default)
    {
        var result = false;
        logger.LogTrace($"Start {nameof(CheckCompanyIdInAms)}");

        if (string.IsNullOrWhiteSpace(companyId))
        {
            throw new ArgumentNullException(nameof(companyId), "CompanyId cannot be null or empty.");
        }

        try
        {
            var payload = BaseRequestDto.Build<CheckCompanyIdRequest>(configuration);
            payload.IdentifierValue = companyId;

            logger.LogTrace("Sending request {@payload} to {destination}", payload, "/api/mps/check");
            var response = await _httpClient.PostAsJsonAsync("/api/mps/check", payload, cancellationToken);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                logger.LogInformation("CompanyId {companyId} found in AMS", companyId);
                result = true;
            }
            var message = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogInformation("CompanyId {companyId} check failed with {statusCode}: {message} ", companyId, response.StatusCode, message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while checking companyId {companyId} in AMS", companyId);
        }

        logger.LogTrace($"Stop {nameof(CheckCompanyIdInAms)}");
        return result;
    }

    public async Task<AdapterResponse<PaidInvoiceDto>> MarkPaid(PaidInvoiceDto invoice, CancellationToken cancellationToken = default)
    {
        logger.LogTrace($"Start {nameof(MarkPaid)}");

        try
        {
            var request = MapInvoiceToMarkPaidRequest(invoice);
            if (request.request.ValidateMarkPaidWithoutElectronicId() is IReadOnlyList<ValidationResult> vr && vr.Any())
            {
                throw new ValidationException($"Mark paid request validation failed: {string.Join(',', vr.Select(v => v.ErrorMessage))}");
            }

            logger.LogTrace("Sending mark paid request {@payload} to {destination}", request.request, request.destination);
            var response = await _httpClient.PostAsJsonAsync(request.destination, request.request, cancellationToken);
            var responseMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogTrace("Response message received: {response}", responseMessage);

            var message = await response.Content.ReadFromJsonAsync<MarkPaidResponse>(cancellationToken);
            logger.LogTrace("Response message received: {@response}", message);
            response.EnsureSuccessStatusCode();

            logger.LogInformation("Mark paid request successful");
            invoice.Message = message.EncodedXml;
            invoice.FiscalizationDateTime = message.FiscalizationTimestamp;
            return AdapterResponse<PaidInvoiceDto>.Success(invoice);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error occured while sending information invoice paid {invoice}", invoice.InvoiceNumber);
            return AdapterResponse<PaidInvoiceDto>.Failure(ex);
        }
        finally
        {
            logger.LogTrace($"End {nameof(MarkPaid)}");
        }
    }

    private (MarkPaidRequest request, string destination) MapInvoiceToMarkPaidRequest(PaidInvoiceDto invoice)
    {
        string destination;

        MarkPaidRequest markPaidRequest = BaseRequestDto.Build<MarkPaidRequest>(configuration);

        markPaidRequest.PaymentDate = invoice!.PaymentDate;
        markPaidRequest.PaymentAmount = invoice.PaymentAmount;

        if (invoice.IsRegistered)
        {
            markPaidRequest.ElectronicId = 3077401; // TODO remove this and uncomment code under mer for demo environemnt doesn't support other companies than dummy ID 3077401 paidObject.CompanyId;
                                                    //if (int.TryParse(paidObject.CompanyId, out var electronicId))
                                                    //    markPaidRequest.ElectronicId = electronicId;

            markPaidRequest.IssueDate = invoice.IssueDate;
            destination = configuration.MarkPaidWithElectronicId;
        }
        else
        {
            markPaidRequest.InternalMark = invoice.InvoiceNumber;
            markPaidRequest.IssueDate = invoice.IssueDate;
            markPaidRequest.SenderIdentifierValue = markPaidRequest.CompanyId;
            markPaidRequest.RecipientIdentifierValue = invoice.CompanyId;
            destination = configuration.MarkPaidWithElectronicId;
        }

        return (markPaidRequest, destination);
    }

    public async Task<RejectedInvoiceDto> MarkRejected(RejectedInvoiceDto invoice, CancellationToken cancellationToken = default)
    {
        using (logger.BeginScope("Marking invoice {InvoiceNumber} as rejected", invoice.InvoiceNumber))
        {
            logger.LogTrace($"Start {nameof(MarkRejected)}");
            if (invoice == null || string.IsNullOrWhiteSpace(invoice.InvoiceNumber))
            {
                logger.LogWarning("Argument cannot be null");
                return new(); // TODO change to result pattern
            }

            try
            {
                var request = MapInvoiceToRejectRequest(invoice);

                logger.LogTrace("Sending reject request {@payload} to {destination}", request.request, request.destination);
                var response = await _httpClient.PostAsJsonAsync(request.destination, request.request, cancellationToken);
                var responseMessage = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogTrace("{statusCode} Response message received: {response}", response.StatusCode, responseMessage);

                var message = await response.Content.ReadFromJsonAsync<MarkPaidResponse>(cancellationToken);
                logger.LogTrace("Response message received: {@response}", message);
                if (response.StatusCode == HttpStatusCode.OK && message?.EncodedXml is not null)
                {
                    logger.LogInformation("Mark reject successful");
                    invoice.Message = message.EncodedXml;
                    invoice.FiscalizationDateTime = message.FiscalizationTimestamp;
                    return invoice;
                }

                return invoice;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occured while sending rejected invoice {@payload}", invoice);
            }

            logger.LogTrace($"End {nameof(MarkRejected)}");
        }

        return invoice;
    }

    private (RejectRequest request, string destination) MapInvoiceToRejectRequest(RejectedInvoiceDto invoice)
    {
        string destination;

        RejectRequest rejectRequest = BaseRequestDto.Build<RejectRequest>(configuration);

        if (invoice.IsRegistered)
        {
            rejectRequest.ElectronicId = 3077401; // TODO remove this and uncomment code under mer for demo environemnt doesn't support other companies than dummy ID 3077401 paidObject.CompanyId;
                                                  //if (int.TryParse(paidObject.CompanyId, out var electronicId))
                                                  //    markPaidRequest.ElectronicId = electronicId;

            rejectRequest.IssueDate = invoice.IssueDate;
            destination = configuration.RejectWithElectronicId;
        }
        else
        {
            rejectRequest.InternalMark = invoice.InvoiceNumber;
            rejectRequest.IssueDate = invoice.IssueDate;
            rejectRequest.SenderIdentifierValue = rejectRequest.CompanyId;
            rejectRequest.RecipientIdentifierValue = invoice.CompanyId;
            destination = configuration.RejectWithoutElectronicId;
        }

        return (rejectRequest, destination);
    }
}
