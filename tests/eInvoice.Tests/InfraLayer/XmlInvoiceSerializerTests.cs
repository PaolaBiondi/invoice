using eInvoice.infra.mojEracun;
using Microsoft.Extensions.Logging;
using Oasis.Names.Specification.Ubl.Schema.Xsd.CommonAggregateComponents._2;
using Oasis.Names.Specification.Ubl.Schema.Xsd.CommonBasicComponents._2;
using Oasis.Names.Specification.Ubl.Schema.Xsd.Invoice._2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Org.XmlUnit.Builder;

namespace eInvoice.Tests.InfraLayer
{
    public class XmlInvoiceSerializerTests
    {
        [Fact]
        public void Serialize_ShouldThrowArgumentNullException_WhenInvoiceIsNull()
        {
            // Arrange
            var serializer = new XmlInvoiceSerializer(null);
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => serializer.Serialize(null));
        }

        [InlineData(TestInvoiceType.PDV0)]
        [InlineData(TestInvoiceType.PDV13)]
        [InlineData(TestInvoiceType.PDV25)]
        [Theory]
        public void Serialize_ShouldReturnSerializedXml_WhenInvoiceIsValid(TestInvoiceType invoiceType)
        {
            // Arrange
            ILogger<XmlInvoiceSerializer> logger = EinvoiceTestFactory.MockLogger<XmlInvoiceSerializer>();

            var serializer = new XmlInvoiceSerializer(logger);
            var invoice = EinvoiceTestFactory.CreateInvoice(invoiceType);
            var expectedXml = EinvoiceTestFactory.ExpectedDocument(invoiceType);

            // Act
            var result = serializer.Serialize(invoice);
            var currentXml = XDocument.Parse(result);

            var diff = DiffBuilder.Compare(expectedXml).WithTest(currentXml)
                .IgnoreWhitespace()
                .Build();

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(currentXml);
            Assert.NotNull(expectedXml);
            Assert.Contains("urn:oasis:names:specification:ubl:schema:xsd:Invoice-2", result);
            Assert.False(diff.HasDifferences(), diff.ToString());
        }
    }
}
