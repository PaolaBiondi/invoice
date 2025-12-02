using eInvoice.domain.Adapters;
using eInvoice.domain.Models;
using eInvoice.domain.Repositories;
using eInvoice.domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace eInvoice.Jobs
{
    internal class FetchPaidAndRejectedInvoiceService : IinvoiceBackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<FetchPaidAndRejectedInvoiceService> _logger;
        private TimeSpan RefreshInterval { get; init; }

        public FetchPaidAndRejectedInvoiceService(IServiceProvider serviceProvider, ILogger<FetchPaidAndRejectedInvoiceService> logger, IOptions<JobsSettings> options)
        {
            RefreshInterval = TimeSpan.FromMinutes(options.Value.RefreshInterval);
            this.serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogTrace("Fetching paid and rejected invoices");
                LastRun = DateTimeOffset.UtcNow;

                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    await mediator.Send<PaidInvoiceDto?, bool>(null, stoppingToken);
                    await mediator.Send<RejectedInvoiceDto?, bool>(null, stoppingToken);

                    //var paidInvoiceRepository = scope.ServiceProvider.GetRequiredService<IPaidInvoiceRepository>();
                    //await ProcessPaidInvoices(paidInvoiceRepository, stoppingToken);

                    //var rejectedInvoiceRepository = scope.ServiceProvider.GetRequiredService<IRejectedInvoiceRepository>();
                    //await ProcessRejectedInvoices(rejectedInvoiceRepository, stoppingToken);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching paid and rejected invoices");
                }

                _logger.LogTrace("Fetched paid and rejected invoices executed");
                await Task.Delay(RefreshInterval, stoppingToken);
            }
        }

        private async Task ProcessPaidInvoices(IPaidInvoiceRepository paidInvoiceRepository, CancellationToken stoppingToken)
        {
            var paidInvoices = await paidInvoiceRepository.GetUnprocessedPaidInvoicesAsync(stoppingToken);
            if (paidInvoices == null)
                return;

            foreach (var paidInvoice in paidInvoices)
            {
                _logger.LogInformation("Paid invoice found: {InvoiceId}", paidInvoice);
                // TODO process paid invoice
                // Simulate work
            }
        }

        private async Task ProcessRejectedInvoices(IRejectedInvoiceRepository rejectedInvoiceRepository, CancellationToken stoppingToken)
        {
            var rejectedInvoices = await rejectedInvoiceRepository.GetUnprocessedRejectedInvoicesAsync(stoppingToken);
            if (rejectedInvoices == null)
                return;

            foreach (var rejectedInvoice in rejectedInvoices)
            {
                _logger.LogInformation("Paid invoice found: {InvoiceId}", rejectedInvoice);
                // TODO process paid invoice
                // Simulate work
            }
        }

        public override bool HealthCheck()
        {
            if (DateTimeOffset.UtcNow.Subtract(LastRun).TotalMinutes < RefreshInterval.TotalMinutes * 2)
                return true;

            _logger.LogError($"{nameof(FetchPaidAndRejectedInvoiceService)} unhealthy");
            return false;
        }
    }
}
