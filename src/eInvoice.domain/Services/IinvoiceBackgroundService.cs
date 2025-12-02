using Microsoft.Extensions.Hosting;

namespace eInvoice.domain.Services
{
    public abstract class IinvoiceBackgroundService: BackgroundService
    {
        protected DateTimeOffset LastRun { get; set; }
        public abstract bool HealthCheck();
    }
}
