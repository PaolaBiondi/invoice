using eInvoice.domain.Services;
using eInvoice.Jobs;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace eInvoice.api.HealthCheck
{
    internal class InfraHealthCheck : IHealthCheck
    {
        private readonly IServiceProvider serviceProvider;

        public InfraHealthCheck(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var isHealthy = serviceProvider.HealthCheck();
            if (isHealthy)
            {
                return Task.FromResult(HealthCheckResult.Healthy("The infra is healthy."));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("The infra is not healthy."));
        }
    }
}
