using eInvoice.domain.Models;
using Microsoft.Extensions.Logging;

namespace eInvoice.UseCases;

internal class PingEinvoiceAdapterHandler: IRequestHandler<PingDto, bool>
{
    private readonly ILogger<PingEinvoiceAdapterHandler> logger;
    private readonly IinvoiceSender sender;
    public PingEinvoiceAdapterHandler(ILogger<PingEinvoiceAdapterHandler> logger, IinvoiceSender sender)
    {
        this.logger = logger;
        this.sender = sender;
    }

    public async Task<bool> Handle(PingDto? request, CancellationToken cancellationToken = default)
    {
        logger.LogTrace("Start {method}", (nameof(Handle)));
        
        var result = await sender.PingAsync();
        
        logger.LogTrace("End {method}", (nameof(Handle)));
        return result;
    }
}
