using eInvoice.domain.Models;
using eInvoice.domain.Repositories;
using Microsoft.Extensions.Logging;

namespace eInvoice.UseCases;

internal class CheckRegistrationHandler : IRequestHandler<CheckCompanyDto?, bool>
{
    private readonly ILogger<CheckRegistrationHandler> logger;
    private readonly IinvoiceSender sender;
    private readonly ICheckCompanyRepository repository;

    public CheckRegistrationHandler(ILogger<CheckRegistrationHandler> logger, IinvoiceSender sender, ICheckCompanyRepository repository)
    {
        this.logger = logger;
        this.sender = sender;
        this.repository = repository;
    }

    public async Task<bool> Handle(CheckCompanyDto? request, CancellationToken cancellationToken = default)
    {
        logger.LogTrace("Start {method}", (nameof(Handle)));

        var companyIds = await repository.CheckCompanyIdsInAmsAsync(cancellationToken);

        if (companyIds != null)
        {
            foreach (var companyId in companyIds)
            {
                var isRegistered = await sender.CheckCompanyIdInAms(companyId, cancellationToken);
                await repository.UpdateCompanyIdAsync(companyId, isRegistered);
            }
        }
        logger.LogTrace("End {method}", (nameof(Handle)));
        return true;
    }
}
