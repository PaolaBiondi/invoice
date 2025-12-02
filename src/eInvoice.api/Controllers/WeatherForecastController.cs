using eInvoice.domain.Adapters;
using eInvoice.domain.Models;
using eInvoice.domain.Repositories;
using eInvoice.infra.efc.Data;
using eInvoice.infra.mojEracun.Scheme;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oasis.Names.Specification.Ubl.Schema.Xsd.Invoice._2;
using System.Net;

namespace eInvoice.api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IinvoiceSender sender;
    private readonly IPaidInvoiceRepository paidInvoiceRepository;
    private readonly IRejectedInvoiceRepository rejectedInvoiceRepository;

    public WeatherForecastController(ILogger<WeatherForecastController> logger,
                                     IinvoiceSender sender,
                                     IPaidInvoiceRepository paidInvoiceRepository,
                                     IRejectedInvoiceRepository rejectedInvoiceRepository)
    {
        _logger = logger;
        this.sender = sender;
        this.paidInvoiceRepository = paidInvoiceRepository;
        this.rejectedInvoiceRepository = rejectedInvoiceRepository;
    }

    [HttpGet("CheckCompanyId/{companyId}")]
    public async Task<IActionResult> CheckCompanyId(string companyId)
    {
        if (string.IsNullOrWhiteSpace(companyId))
            return BadRequest();

        var result = await sender.CheckCompanyIdInAms(companyId);
        if (result)
        {
            return Ok();
        }

        return BadRequest();
    }

    [HttpPost("MarkPaid")]
    public async Task<IActionResult> MarkPaid([FromBody] string invoice, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(invoice))
            return BadRequest("Invoice cannot be null or empty.");

        var payload = await paidInvoiceRepository.GetPaidInvoiceAsync(invoice, cancellationToken);

        if (payload == null)
        {
            return BadRequest("Paid invoice data not found.");
        }

        var result = await sender.MarkPaid(payload, cancellationToken);
        if (!string.IsNullOrWhiteSpace(result.Message) || result.FiscalizationDateTime is not null)
        {
            if (await paidInvoiceRepository.UpdatePaidInvoiceAsync(payload, cancellationToken))
                return Ok();
        }
        return BadRequest("Failed to mark invoice as paid.");
    }

    [HttpPost("RejectInvoice")]
    public async Task<IActionResult> MarkRejected([FromBody] string invoice, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(invoice))
            return BadRequest("Invoice cannot be null or empty.");

        var payload = await rejectedInvoiceRepository.GetRejectedInvoiceAsync(invoice, cancellationToken);

        if (payload == null)
        {
            return BadRequest("Rejected invoice data not found.");
        }

        var result = await sender.MarkRejected(payload, cancellationToken);
        if (!string.IsNullOrWhiteSpace(result.Message) || result.FiscalizationDateTime is not null)
        {
            if (await rejectedInvoiceRepository.UpdateRejectedInvoiceAsync(payload, cancellationToken))
                return Ok();
        }

        return BadRequest("Reject request failed");
    }
}
