using eInvoice.domain.Common;
using eInvoice.domain.Models;
using eInvoice.domain.Repositories;
using Microsoft.Extensions.Logging;

namespace eInvoice.UseCases;

internal class SendInvoiceHandler : IRequestHandler<ReadyInvoiceDto?, bool>
{
    private readonly ILogger<SendInvoiceHandler> _logger;
    private readonly IInvoiceRepository repo;
    private readonly IinvoiceSender sender;

    public SendInvoiceHandler(ILogger<SendInvoiceHandler> logger, IInvoiceRepository repo, IinvoiceSender sender)
    {
        this._logger = logger;
        this.repo = repo;
        this.sender = sender;
    }

    public async Task<bool> Handle(ReadyInvoiceDto? request, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Start {method}", (nameof(Handle)));

        if (request?.InvoiceNumber is not null)
        {
            // request to send specific invoice
            await SendInvoiceAsync(request.InvoiceNumber, cancellationToken);
        }
        else
        {
            // pool request to send all ready invoices
            await SendInvoicesAsync(cancellationToken);
        }

        _logger.LogTrace("End {method}", (nameof(Handle)));
        return true;
    }

    private async Task SendInvoiceAsync(string invoice, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Ready invoice found: {InvoiceId}", invoice);
        var result = await sender.SendInvoiceAsync(invoice, true, cancellationToken);
        
        // TODO if result is successful, update invoice status in repository
        if (!result.Errors.Any() && result.Value is not null)
        {
            _logger.LogInformation("Invoice {InvoiceId} sent successfully.", invoice);
            await repo.UpdateInvoiceStatus(result.Value, cancellationToken);
        }
        else
        {
            _logger.LogError("Failed to send invoice {InvoiceId}", invoice);
        }
    }

    private async Task SendInvoicesAsync(CancellationToken cancellationToken)
    {
        var invoices = await repo.GetReadyInvoicesAsync();
        foreach (var invoice in invoices)
        {
            await SendInvoiceAsync(invoice.InvoiceNumber, cancellationToken);
        }
    }

}
