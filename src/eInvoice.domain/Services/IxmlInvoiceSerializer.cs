using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eInvoice.domain.Services
{
    public interface IxmlInvoiceSerializer<in TInvoice> where TInvoice : class
    {
        string Serialize(TInvoice invoice);
        //if (obj == null)
        //    throw new ArgumentNullException(nameof(obj));
        //var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        //using (var stringWriter = new System.IO.StringWriter())
        //{
        //    xmlSerializer.Serialize(stringWriter, obj);
        //    return stringWriter.ToString();
        //}
    }
}
