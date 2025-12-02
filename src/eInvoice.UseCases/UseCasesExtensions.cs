using eInvoice.domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace eInvoice.UseCases;

public static class UseCasesExtensions
{
    public static IServiceCollection AddUseCasesServices(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<ReadyInvoiceDto?, bool>, SendInvoiceHandler>();
        services.AddScoped<IRequestHandler<PingDto, bool>, PingEinvoiceAdapterHandler>();
        services.AddScoped<IRequestHandler<PaidInvoiceDto?, bool>, PaidInvoiceHandler>();
        services.AddScoped<IRequestHandler<RejectedInvoiceDto?, bool>, RejecredInvoiceHandler>();
        services.AddScoped<IRequestHandler<PurgeDto, bool>, PurgingInvoiceHandler>();
        services.AddScoped<IRequestHandler<CheckCompanyDto?, bool>, CheckRegistrationHandler>();
        return services;
    }
}
