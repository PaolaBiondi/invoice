using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Oasis.Names.Specification.Ubl.Schema.Xsd.Invoice._2
{
    public partial class InvoiceType
    {
        [XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string SchemaLocation { get; set; } = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2 ../xsd/ubl/maindoc/UBL-Invoice-2.1.xsd";
    }
}
