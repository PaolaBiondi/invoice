using eInvoice.api.HealthCheck;
using eInvoice.domain.Adapters;
using eInvoice.domain.Repositories;
using eInvoice.domain.Services;
using eInvoice.infra.efc.Data;
using eInvoice.infra.efc.Repositories;
using eInvoice.infra.mojEracun;
using eInvoice.infra.mojEracun.Scheme;
using eInvoice.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Oasis.Names.Specification.Ubl.Schema.Xsd.Invoice._2;
using eInvoice.UseCases;

namespace eInvoice.api
{
    public static class InfraExtensions
    {
        public static IServiceCollection AddInfraServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<EinvoiceSettings>()
                .BindConfiguration(nameof(EinvoiceSettings))
                .ValidateDataAnnotations();

            services.AddScoped<IMediator, EInvoiceMediator>();

            services.AddJobsServices();
            services.AddUseCasesServices();

            services.RegisterMojEracunServices();

            services.RegisterEfcServices(configuration);

            services.AddHealthChecks()
                    .AddCheck<InfraHealthCheck>("Infra Health Check");

            return services;
        }

        public static WebApplicationBuilder AddLogger(this WebApplicationBuilder builder)
        {
            Serilog.Debugging.SelfLog.Enable(msg =>
            {
                Console.WriteLine(msg);
                System.IO.File.AppendAllText("Logs/serilog-selflog.txt", msg);
            });

            Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration)
                    .CreateLogger();
            builder.Services.AddSerilog();

            builder.Host.UseSerilog();

            return builder;
        }
    }
}
