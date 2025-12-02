using eInvoice.domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace eInvoice.domain.Models
{
    public class Attachment
    {
        public string InvoiceId { get; set; } = null!;
        /// <summary>
        /// Collection of attachments for an invoice
        /// <para>
        /// <description>fileName naming convention</description>
        /// <list type="bullet">
        /// <item><description><c>{invoiceId}.pdf</c> - Main PDF snapshot of the invoice.</description></item>
        /// <item><description><c>ATT_{invoiceId}.pdf</c> - Additional PDF attachment, if present.</description></item>
        /// <item><description><c>ATT_{invoiceId}.xlsx</c> - Additional XLSX attachment, if present.</description></item>
        /// </list>
        /// </para>
        ///  Use <see cref="GetPdfFileName(string)"/> and related helpers to construct filenames.
        /// </summary>
        public ICollection<(string fileName, string content, MimeCodeQualifier memeCode)>? StrBase64 { get; set; }

        /// <summary>Returns the main iPDF filename for the given invoice ID.</summary>
        public static string GetPdfFileName(string invoiceId) => $"{invoiceId}.pdf";

        /// <summary>Returns the additional PDF attachment filename for the given invoice ID.</summary>
        public static string GetAttachmentPdfFileName(string invoiceId) => $"ATT_{invoiceId}.pdf";

        /// <summary>Returns the additional XLSX attachment filename for the given invoice ID.</summary>
        public static string GetAttachmentXlsxFileName(string invoiceId) => $"ATT_{invoiceId}.xlsx";
    }
}
