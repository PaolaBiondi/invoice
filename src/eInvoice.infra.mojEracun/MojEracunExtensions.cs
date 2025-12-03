using eInvoice.domain.Services;
using eInvoice.infra.mojEracun.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Oasis.Names.Specification.Ubl.Schema.Xsd.Invoice._2;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public static bool Check(this MarkPaidRequest request, Func<MarkPaidRequest, bool> validate)
        {
            return validate(request);
        }

        public static IReadOnlyList<ValidationResult> ValidateMarkPaidWithoutElectronicId(this MarkPaidRequest request)
        {
            var validation = new List<ValidationResult>();

            if (request is null)
            {
                validation.Add(new ValidationResult("Request cannot be null"));
                return validation;
            }
            if (string.IsNullOrWhiteSpace(request.InternalMark))
            {
                validation.Add(new ValidationResult($"{nameof(request.InternalMark)} Null or empty"));
            }
            if (string.IsNullOrWhiteSpace(request.SenderIdentifierValue))
            {
                validation.Add(new ValidationResult("Request cannot be null"));
            }
            if (string.IsNullOrWhiteSpace(request.RecipientIdentifierValue))
            {
                validation.Add(new ValidationResult("Request cannot be null"));
            }
            if (request.PaymentAmount <= 0)
            {
                validation.Add(new ValidationResult("Request cannot be null"));
            }
            if (request.PaymentDate == default)
            {
                validation.Add(new ValidationResult("Request cannot be null"));
            }
            if (request.IssueDate == default)
            {
                validation.Add(new ValidationResult("Request cannot be null"));
            }

            return validation;
        }
    }
}
