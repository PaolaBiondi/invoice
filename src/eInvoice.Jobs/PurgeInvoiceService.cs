using eInvoice.domain.Adapters;
using eInvoice.domain.Models;
using eInvoice.domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace eInvoice.Jobs
{
    internal class PurgeInvoiceService : IinvoiceBackgroundService
    {
        private readonly ILogger<PurgeInvoiceService> _logger;
        private readonly IServiceProvider serviceProvider;
        private readonly JobsSettings jobsSettings;

        public PurgeInvoiceService(ILogger<PurgeInvoiceService> logger, IServiceProvider serviceProvider, IOptions<JobsSettings> options)
        {
            this._logger = logger;
            this.serviceProvider = serviceProvider;
            this.jobsSettings = options.Value;
        }

        public override bool HealthCheck()
        {
            if (DateTimeOffset.UtcNow.Subtract(LastRun).TotalHours < 25)
                return true;

            _logger.LogError($"{nameof(PurgeInvoiceService)} unhealthy");
            return false;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Purging started..");
                LastRun = DateTimeOffset.UtcNow;
                // occurs every day at 6:00 AM
                var nextRun = LastRun.Date.AddDays(1).AddHours(6);
                var delay = nextRun - LastRun;

                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    var purgeRequest = new PurgeDto
                    {
                        Months = jobsSettings.PurgeAfterMonths
                    };

                    await mediator.Send<PurgeDto, bool>(purgeRequest, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occured while purging");
                }

                _logger.LogInformation("Purge executed. Next will occur at {nextRun}", nextRun);

                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
