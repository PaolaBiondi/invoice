using eInvoice.domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eInvoice.Jobs
{
    public static class JobsExtensions
    {
        public static IServiceCollection AddJobsServices(this IServiceCollection services)
        {
            services.AddOptions<JobsSettings>()
                    .BindConfiguration(nameof(JobsSettings));

            services.AddHostedService<FetchInvoiceService>();
            services.AddHostedService<PurgeInvoiceService>();
            services.AddHostedService<CheckRegistrationStatusService>();
            services.AddHostedService<FetchPaidAndRejectedInvoiceService>();

            return services;
        }

        public static bool HealthCheck(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var jobs = scope.ServiceProvider.GetServices<IHostedService>().OfType<IinvoiceBackgroundService>();
            foreach (var job in jobs)
            {
                if (!job.HealthCheck())
                    return false;
            }
            return true;
        }
    }
}
