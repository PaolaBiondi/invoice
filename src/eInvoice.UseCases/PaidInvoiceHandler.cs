using eInvoice.domain.Models;
using Microsoft.Extensions.Logging;

namespace eInvoice.UseCases;

internal class PaidInvoiceHandler : IRequestHandler<PaidInvoiceDto?, bool>
{
    private readonly ILogger<PaidInvoiceHandler> logger;
    private readonly IinvoiceSender sender;
    public PaidInvoiceHandler(ILogger<PaidInvoiceHandler> logger, IinvoiceSender sender)
    {
        this.logger = logger;
        this.sender = sender;
    }

    public async Task<bool> Handle(PaidInvoiceDto? request, CancellationToken cancellationToken = default)
    {
        logger.LogTrace("Start {method}", (nameof(Handle)));

        var result = true; // await sender.MarkPaid(request?.InvoiceNumber ?? string.Empty, cancellationToken);
        
        logger.LogTrace("End {method}", (nameof(Handle)));
        return result;
    }
}
