using eInvoice.domain.Models;
using eInvoice.domain.Repositories;
using Microsoft.Extensions.Logging;

namespace eInvoice.UseCases;

internal class PurgingInvoiceHandler : IRequestHandler<PurgeDto, bool>
{
    private readonly ILogger<PurgingInvoiceHandler> logger;
    private readonly IPurgeRepository repo;

    public PurgingInvoiceHandler(ILogger<PurgingInvoiceHandler> logger, IPurgeRepository repo)
    {
        this.logger = logger;
        this.repo = repo;
    }

    public async Task<bool> Handle(PurgeDto request, CancellationToken cancellationToken = default)
    {
        logger.LogTrace("Start {method}", (nameof(Handle)));

        var result = await repo.PurgeAsync(request.Months, cancellationToken);

        logger.LogTrace("End {method}", (nameof(Handle)));
        return result;
    }
}
