using eInvoice.domain.Repositories;
using eInvoice.infra.efc.Data;
using Microsoft.Extensions.Logging;

namespace eInvoice.infra.efc.Repositories
{
    internal class PurgeRepository : IPurgeRepository
    {
        private readonly ILogger<PurgeRepository> logger;
        private readonly BillingContext billingContext;

        public PurgeRepository(ILogger<PurgeRepository> logger, BillingContext billingContext)
        {
            this.logger = logger;
            this.billingContext = billingContext;
        }

        public async Task<bool> PurgeAsync(ushort months, CancellationToken cancellationToken = default)
        {
            try
            {
                await billingContext.InvoiceDetails.Where(i => i.Created < DateTimeOffset.UtcNow.AddMonths(-months))
                                            .ExecuteDeleteAsync();

                await billingContext.Database.ExecuteSqlAsync($"DELETE FROM Logs where TimeStamp < DATEADD(MONTH, -{months}, GETUTCDATE())");

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError("Error occured while pruging {message}", ex.Message);
            }

            return false;
        }
    }
}
