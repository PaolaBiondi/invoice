using eInvoice.domain.Repositories;
using eInvoice.infra.efc.Data;
using eInvoice.infra.efc.Infra.Models;
using Microsoft.Extensions.Logging;

namespace eInvoice.infra.efc.Repositories
{
    internal class CheckCompanyRepository : ICheckCompanyRepository
    {
        private readonly ILogger<CheckCompanyRepository> logger;
        private readonly BillingContext context;

        public CheckCompanyRepository(ILogger<CheckCompanyRepository> logger, BillingContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        /// <summary>
        /// Gets all company ids from EdiInvoicingToDetails table where IsRegistered is null
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyList<string>?> CheckCompanyIdsInAmsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var list = await context.CustomerDetails.Where(c => c.IsRegistered == null)
                                                              .Select(c => c.CompanyId)
                                                              .ToListAsync();

                return list?.AsReadOnly();
            }
            catch (Exception ex)
            {
                logger.LogError("Error occured while checking company ids in ams {message}", ex.Message);
                return null;
            }
        }

        public async Task UpdateCompanyIdAsync(string companyId, bool isRegistered)
        {
            try
            {
                var company = await context.CustomerDetails.Where(c => c.CompanyId == companyId)
                                                                 .FirstOrDefaultAsync();

                if (company is null)
                {
                    logger.LogInformation("CoompanyId {companyId} not found in {table}", companyId, nameof(CustomerDetails));
                    return;
                }

                company.IsRegistered = isRegistered;
                company.Updated = DateTimeOffset.Now;
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message,"Error occured while updating company id {message}", companyId);
            }
        }
    }
}
