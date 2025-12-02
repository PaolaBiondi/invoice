using eInvoice.domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Oasis.Names.Specification.Ubl.Schema.Xsd.Invoice._2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eInvoice.infra.mojEracun
{
    public static class MojEracunExtensions
    {
        public static IServiceCollection RegisterMojEracunServices(this IServiceCollection services)
        {
            services.AddHttpClient<IinvoiceSender, InvoiceSender>();
            services.AddTransient<IxmlInvoiceSerializer<InvoiceType>, XmlInvoiceSerializer>();

            return services;
        }
    }
}
