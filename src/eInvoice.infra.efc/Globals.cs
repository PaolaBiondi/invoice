global using Microsoft.EntityFrameworkCore;
using eInvoice.domain.Repositories;
using eInvoice.infra.efc.Data;
using eInvoice.infra.efc.Models;
using eInvoice.infra.efc.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class  Globlas
{
    public static IServiceCollection RegisterEfcServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IInvoiceRepository, InvoiceRepository>();
        services.AddTransient<IPaidInvoiceRepository, PaidInvoicesRepositroy>();
        services.AddTransient<IRejectedInvoiceRepository, RejectedInvoiceRepository>();
        services.AddTransient<IPurgeRepository, PurgeRepository>();
        services.AddTransient<ICheckCompanyRepository, CheckCompanyRepository>();

        services.AddDbContext<BillingContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("BillingConnection")));

        services.AddHealthChecks()
            .AddDbContextCheck<BillingContext>();
        // Register your EF Core related services here
        return services;
    }

}