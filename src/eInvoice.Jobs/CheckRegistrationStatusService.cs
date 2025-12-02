using eInvoice.domain.Adapters;
using eInvoice.domain.Models;
using eInvoice.domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace eInvoice.Jobs
{
    internal class CheckRegistrationStatusService : IinvoiceBackgroundService
    {
        private readonly ILogger<CheckRegistrationStatusService> _logger;
        private readonly IServiceProvider serviceProvider;
        private TimeSpan RefreshInterval { get; init; }

        public CheckRegistrationStatusService(ILogger<CheckRegistrationStatusService> logger,
                                              IServiceProvider serviceProvider,
                                              IOptions<JobsSettings> configuration)
        {
            RefreshInterval = TimeSpan.FromMinutes(configuration.Value.RefreshInterval);
            this._logger = logger;
            this.serviceProvider = serviceProvider;
        }

        public override bool HealthCheck()
        {
            if (DateTimeOffset.UtcNow.Subtract(LastRun).TotalHours < RefreshInterval.TotalMinutes * 2)
                return true;

            _logger.LogError($"{nameof(CheckRegistrationStatusService)} unhealthy");
            return false;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogTrace("CheckCompanyIdsService started..");
                LastRun = DateTimeOffset.UtcNow;

                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    await mediator.Send<CheckCompanyDto?, bool>(null!, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking company IDs in AMS");
                }

                _logger.LogTrace("CheckCompanyIdsService executed.");
                await Task.Delay(RefreshInterval, stoppingToken);
            }
        }
    }
}
