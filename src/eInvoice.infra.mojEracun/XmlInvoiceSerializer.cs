using eInvoice.domain.Services;
using Microsoft.Extensions.Logging;
using Oasis.Names.Specification.Ubl.Schema.Xsd.Invoice._2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace eInvoice.infra.mojEracun
{
    internal class XmlInvoiceSerializer : IxmlInvoiceSerializer<InvoiceType>
    {
        private readonly ILogger<XmlInvoiceSerializer> logger;

        public XmlInvoiceSerializer(ILogger<XmlInvoiceSerializer> logger)
        {
            this.logger = logger;
        }

        public string Serialize(InvoiceType invoice)
        {
            if (invoice == null)
            {
                throw new ArgumentNullException(nameof(invoice), "Invoice cannot be null.");
            }

            var serializer = new XmlSerializer(typeof(InvoiceType));

            // Define all required namespaces
            var ns = new XmlSerializerNamespaces();
            ns.Add(String.Empty, "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2"); // default
            ns.Add("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            ns.Add("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
            ns.Add("cct", "urn:un:unece:uncefact:data:specification:CoreComponentTypeSchemaModule:2");
            ns.Add("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
            ns.Add("hrextac", "urn:hzn.hr:schema:xsd:HRExtensionAggregateComponents-1");
            ns.Add("p3", "urn:oasis:names:specification:ubl:schema:xsd:UnqualifiedDataTypes-2");
            ns.Add("sac", "urn:oasis:names:specification:ubl:schema:xsd:SignatureAggregateComponents-2");
            ns.Add("sig", "urn:oasis:names:specification:ubl:schema:xsd:CommonSignatureComponents-2");
            ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

            //using var stringWriter = new StringWriter();
            using var memoryStream = new MemoryStream();
            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Indent = true,
                OmitXmlDeclaration = false
            };
            using var xmlWriter = XmlWriter.Create(memoryStream, settings);
            serializer.Serialize(xmlWriter, invoice, ns);
            var serializedMessage = Encoding.UTF8.GetString(memoryStream.ToArray());

            return serializedMessage;
        }
    }
}
