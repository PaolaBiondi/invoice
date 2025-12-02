using eInvoice.domain.Models;
using Microsoft.Extensions.Logging;

namespace eInvoice.UseCases;

internal class RejecredInvoiceHandler: IRequestHandler<RejectedInvoiceDto?, bool>
{
    private readonly ILogger<RejecredInvoiceHandler> logger;
    private readonly IinvoiceSender sender;
    public RejecredInvoiceHandler(ILogger<RejecredInvoiceHandler> logger, IinvoiceSender sender)
    {
        this.logger = logger;
        this.sender = sender;
    }
    public async Task<bool> Handle(RejectedInvoiceDto? request, CancellationToken cancellationToken = default)
    {
        logger.LogTrace("Start {method}", (nameof(Handle)));
        var result = true; // await sender.MarkRejected(request?.InvoiceNumber ?? string.Empty, cancellationToken);
        
        logger.LogTrace("End {method}", (nameof(Handle)));
        return result;
    }
}
