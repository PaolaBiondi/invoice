using eInvoice.domain.Adapters;
using eInvoice.domain.Models;
using eInvoice.domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace eInvoice.Jobs
{
    internal class FetchInvoiceService : IinvoiceBackgroundService
    {
        private readonly ILogger<FetchInvoiceService> _logger;
        private readonly IServiceProvider serviceProvider;

        private TimeSpan RefreshInterval { get; init; }

        public FetchInvoiceService(ILogger<FetchInvoiceService> logger, IServiceProvider serviceProvider, IOptions<JobsSettings> options)
        {
            RefreshInterval = TimeSpan.FromMinutes(options.Value.RefreshInterval);
            _logger = logger;
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Fetching ready invoices");
                LastRun = DateTimeOffset.UtcNow;

                try
                {
                    using var serviceScope = serviceProvider.CreateScope();
                    var mediator = serviceScope.ServiceProvider.GetRequiredService<IMediator>();

                    await mediator.Send<ReadyInvoiceDto, bool>(null!, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching invoices");
                }

                _logger.LogInformation("Fetched invoices");

                await Task.Delay(RefreshInterval, stoppingToken);
            }
        }

        public override bool HealthCheck()
        {
            if (DateTimeOffset.UtcNow.Subtract(LastRun).TotalMinutes < RefreshInterval.TotalMinutes * 2)
                return true;

            _logger.LogError($"{nameof(FetchInvoiceService)} unhealthy");
            return false;
        }
    }
}
